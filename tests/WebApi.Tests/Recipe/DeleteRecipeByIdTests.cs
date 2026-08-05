using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Secutiry;
using MyRecipeBook.Exception;
using Shouldly;
using WebApi.Tests.InLineData;
using WebApi.Tests.Resource;

namespace WebApi.Tests.Recipe;

public class DeleteRecipeByIdTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/recipes";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public DeleteRecipeByIdTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var dbContext = await _factory.GetDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.Recipes.AddAsync(recipe);
        await dbContext.SaveChangesAsync();

        var token = JwtTokenBuilder.Build(user, _factory.GetJwtSigningKey(), _factory.GetJwtExpirationTimeMinutes());
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.DeleteAsync($"{REQUEST_URI}/{recipe.Id}");
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenRecipeNotFound(string culture)
    {
        var (user, _) = UserBuilder.Build();

        var dbContext = await _factory.GetDbContext();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var token = JwtTokenBuilder.Build(user, _factory.GetJwtSigningKey(), _factory.GetJwtExpirationTimeMinutes());

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _httpClient.DeleteAsync($"{REQUEST_URI}/{Guid.CreateVersion7()}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await System.Text.Json.JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATION_RECIPE_NOT_FOUND", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }
}