using CommonTestsUtilities.Entities;
using MyRecipeBook.Communication.Requets.VerificationCode;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InLineData;
using WebApi.Tests.Resource;
using Microsoft.EntityFrameworkCore;
using CommonTestsUtilities.Requests.Member;

namespace WebApi.Tests.PasswordRecovery;

public class ResetPasswordTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/authentication/password-recovery/reset";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public ResetPasswordTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var dbContext = await _factory.GetDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var verificationCode = new VerificationCode
        {
            Code = "123456",
            Type = VerificationCodeType.PasswordRecovery,
            UserId = user.Id
        };

        await dbContext.VerificationCodes.AddAsync(verificationCode);
        await dbContext.SaveChangesAsync();

        var request = new RequestResetPasswordJson
        {
            Email = user.Email,
            Code = verificationCode.Code,
            NewPassword = "NewPassword@123"
        };

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var codeExists = await dbContext.VerificationCodes.AnyAsync(code => code.Id == verificationCode.Id);
        codeExists.ShouldBeFalse();
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task ShouldThrowError_WhenUserDoesNotExist(string culture)
    {
        var request = RequestResetPasswordJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager
            .GetString("VERIFICATION_CODE_INVALID", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedMessage));
        });
    }
}
