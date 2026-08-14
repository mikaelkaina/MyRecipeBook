using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using WebApi.Tests.Resource;

namespace WebApi.Tests.PasswordRecovery;

public class RequestPasswordRecoveryCodeTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/authentication/password-recovery";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public RequestPasswordRecoveryCodeTests(CustomWebApplicationFactory factory)
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

        var request = RequestPasswordRecoveryJsonBuilder.Build();
        request.Email = user.Email;

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Success_ShouldDoNothing_WhenEmailNotFound()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }
}