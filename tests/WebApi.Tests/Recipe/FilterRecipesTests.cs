using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Requests.Recipe;
using CommonTestsUtilities.Secutiry;
using MyRecipeBook.Communication.Requets.Recipe;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.Resource;

namespace WebApi.Tests.Recipe;

public class FilterRecipesTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/recipes/filter";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public FilterRecipesTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.BuildMany(user, count: 3);

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

        var recipeToMatch = recipes.First();

        var request = new RequestFilterRecipesJson
        {
            SearchTerm = recipeToMatch.Title,
            CookTime = (MyRecipeBook.Communication.Enums.CookTime)recipeToMatch.CookTime,
            DishTypes = recipeToMatch.DishTypes
                .Select(dt => (MyRecipeBook.Communication.Enums.DishType)dt.Type)
                .ToList()
        };

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var recipesJson = responseData.RootElement.GetProperty("recipes").EnumerateArray().ToList();

        recipesJson.ShouldNotBeEmpty();
        recipesJson.ShouldContain(r => r.GetProperty("title").GetString() == recipeToMatch.Title);
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenNoRecipesMatchFilter()
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

        var request = RequestFilterRecipesJsonBuilder.Build();

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var recipesJson = responseData.RootElement.GetProperty("recipes").EnumerateArray().ToList();

        recipesJson.ShouldBeEmpty();
    }

    [Fact]
    public async Task Success_ShouldReturnAllRecipes_WhenRequestIsNull()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.BuildMany(user, count: 3);

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

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, (RequestFilterRecipesJson?)null);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var recipesJson = responseData.RootElement.GetProperty("recipes").EnumerateArray().ToList();

        recipesJson.Count.ShouldBe(3);
    }
}
