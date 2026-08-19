using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Files;
using CommonTestsUtilities.Requests.Recipe;
using CommonTestsUtilities.Secutiry;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApi.Tests.InLineData;
using WebApi.Tests.Resource;

namespace WebApi.Tests.Recipe;

public class RecipeRegisterTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string REQUEST_URI = "/api/recipes";
    private const string FILE_FIELD_NAME = "recipeIllustration";
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory _factory;

    public RecipeRegisterTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Success_WithoutImage()
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

        var request = RequestRecipeJsonBuilder.Build();

        var response = await _httpClient.PostAsync(REQUEST_URI, BuildFormData(request));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetString().ShouldNotBeNullOrEmpty();
        responseData.RootElement.GetProperty("title").GetString().ShouldBe(request.Title);
    }

    [Fact]
    public async Task Success_WithImage()
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

        var request = RequestRecipeJsonBuilder.Build();

        var response = await _httpClient.PostAsync(REQUEST_URI, BuildFormData(request, FileBuilder.GetJpeg(), FILE_FIELD_NAME));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetString().ShouldNotBeNullOrEmpty();
        responseData.RootElement.GetProperty("title").GetString().ShouldBe(request.Title);
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Error_WhenImageIsTxt(string culture)
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

        var request = RequestRecipeJsonBuilder.Build();

        var response = await _httpClient.PostAsync(REQUEST_URI, BuildFormData(request, FileBuilder.GetTxt(), FILE_FIELD_NAME));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATON_ONLY_IMAGES_ACCEPTED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task Validate_ShouldBeAnErrorResponse_WhenTitleIsEmpty(string culture)
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

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var response = await _httpClient.PostAsync(REQUEST_URI, BuildFormData(request));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager
            .GetString("VALIDATION_TITLE_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString()!.Equals(expectedErrorMessage));
        });
    }

    private static MultipartFormDataContent BuildFormData(
    MyRecipeBook.Communication.Requets.Recipe.RequestRecipeJson request,
    Stream? file = null,
    string? fileFieldName = null)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(request.Title ?? string.Empty), "title");
        formData.Add(new StringContent(((int)request.CookTime).ToString()), "cookTime");

        foreach (var ingredient in request.Ingredients)
            formData.Add(new StringContent(ingredient), "ingredients");

        foreach (var dishType in request.DishTypes)
            formData.Add(new StringContent(((int)dishType).ToString()), "dishTypes");

        var instructionOrder = 0;
        foreach (var instruction in request.Instructions)
        {
            formData.Add(new StringContent(instruction.Order.ToString()), $"instructions[{instructionOrder}].order");
            formData.Add(new StringContent(instruction.Description ?? string.Empty), $"instructions[{instructionOrder}].description");
            instructionOrder++;
        }

        if (file is not null && fileFieldName is not null)
        {
            var fileContent = new StreamContent(file);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            formData.Add(fileContent, fileFieldName, "file");
        }

        return formData;
    }
}