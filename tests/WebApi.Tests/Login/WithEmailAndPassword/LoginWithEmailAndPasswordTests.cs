using CommonTestsUtilities.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Tests.InLineData;

namespace WebApi.Tests.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string REQUEST_URI = "/authentication";
    private readonly HttpClient _httpClient;

    public LoginWithEmailAndPasswordTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Theory]
    [ClassData(typeof(CultureInLineData))]
    public async Task ShouldThrowException_WhenUserDontExist(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);

        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("errors").EnumerateArray();

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedErrorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_LOGIN_INVALID",
            new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()
            && error.GetString()!.Equals(expectedErrorMessage));
        });
    }
}
