using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Requests;
using CommonTestsUtilities.Secutiry;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InLineData;
using WebApi.Tests.Resource;

namespace WebApi.Tests.User.Profile;

public class UpdateUserProfileTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/users/profile";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public UpdateUserProfileTests(CustomWebApplicationFactory factory)
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
            new AuthenticationHeaderValue("Bearer", token);

        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await _httpClient.PutAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNameIsEmpty(string culture)
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
            new AuthenticationHeaderValue("Bearer", token);

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var response = await _httpClient.PutAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenEmailAlreadyExists(string culture)
    {
        var (user, _) = UserBuilder.Build();
        var (anotherUser, _) = UserBuilder.Build();

        var dbContext = await _factory.GetDbContext();
        await dbContext.Users.AddRangeAsync(user, anotherUser);
        await dbContext.SaveChangesAsync();

        var token = JwtTokenBuilder.Build(
            user,
            _factory.GetJwtSigningKey(),
            _factory.GetJwtExpirationTimeMinutes());

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = anotherUser.Email;

        var response = await _httpClient.PutAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATION_EMAIL_ALREADY_EXISTS", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }
}
