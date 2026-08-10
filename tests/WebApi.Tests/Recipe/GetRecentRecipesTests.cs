using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Secutiry;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApi.Tests.Resource;

namespace WebApi.Tests.Recipe;

public class GetRecentRecipesTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/recipes/recent";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public GetRecentRecipesTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.BuildMany(user, count: 2);

        var dbContext = await _factory.GetDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.Recipes.AddRangeAsync(recipes);
        await dbContext.SaveChangesAsync();

        var token = JwtTokenBuilder.Build(
            user,
            _factory.GetJwtSigningKey(),
            _factory.GetJwtExpirationTimeMinutes());

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(REQUEST_URI);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var recipesJson = responseData.RootElement.GetProperty("recipes").EnumerateArray().ToList();

        recipesJson.Count.ShouldBe(2);
        recipesJson.ShouldAllBe(r =>
            recipes.Any(recipe =>
                recipe.Id.ToString() == r.GetProperty("id").GetString() &&
                recipe.Title == r.GetProperty("title").GetString()));
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenNoRecipesExist()
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

        var response = await _httpClient.GetAsync(REQUEST_URI);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var recipesJson = responseData.RootElement.GetProperty("recipes").EnumerateArray().ToList();

        recipesJson.ShouldBeEmpty();
    }
}
