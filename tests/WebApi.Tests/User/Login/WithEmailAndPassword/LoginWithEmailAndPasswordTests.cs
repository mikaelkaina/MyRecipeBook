using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Requests;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InLineData;
using WebApi.Tests.Resource;

namespace WebApi.Tests.User.Login.WithEmailAndPassword;


public class LoginWithEmailAndPasswordTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/authentication";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public LoginWithEmailAndPasswordTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        var (user, plainPassword) = UserBuilder.Build();

        var dbContext = await _factory.GetDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var request = new RequestLoginJson
        {
            Email = user.Email,
            Password = plainPassword
        };

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(user.Name);
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldBeEmpty();
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenCredentialsAreInvalid(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATION_LOGIN_INVALID", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }
}
