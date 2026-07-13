using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Secutiry;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.InLineData;
using WebApi.Tests.Resource;

namespace WebApi.Tests.User.Profile;

public class GetUserProfileTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/users";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public GetUserProfileTests(CustomWebApplicationFactory factory)
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

        var token = JwtTokenBuilder.Build(
            user,
            _factory.GetJwtSigningKey(),
            _factory.GetJwtExpirationTimeMinutes());

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(REQUEST_URI);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(user.Name);
        responseData.RootElement.GetProperty("email").GetString().ShouldBe(user.Email);
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenTokenIsMissing(string culture)
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _httpClient.GetAsync(REQUEST_URI);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATION_ACCESS_TOKEN_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }
}