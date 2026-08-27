using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Files;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories;
using CommonTestsUtilities.Repositories.Formula;
using CommonTestsUtilities.Requests.Recipe;
using CommonTestsUtilities.Storage;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;
using System.Net;

namespace UseCase.Tests.Recipe.Register;

public class RecipeRegisterUseCaseTests
{
    static RecipeRegisterUseCaseTests()
    {
        MapsterConfiguration.Configure();
    }

    [Fact]
    public async Task Success_WithoutImage()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request, recipeIllustration: null);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Title.ShouldBe(request.Title);
        result.ImageUrl.ShouldBeEmpty();
    }

    [Fact]
    public async Task Success_WhenImageIsPng()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request, recipeIllustration: FileBuilder.GetPng());

        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
        result.ImageUrl.ShouldBe(IStorageServiceBuilder.FakeUrl);
    }

    [Fact]
    public async Task Success_WhenImageIsJpeg()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request, recipeIllustration: FileBuilder.GetJpeg());

        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
        result.ImageUrl.ShouldBe(IStorageServiceBuilder.FakeUrl);
    }

    [Fact]
    public async Task Error_WhenImageIsTxt()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request, recipeIllustration: FileBuilder.GetTxt()).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATON_ONLY_IMAGES_ACCEPTED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenTitleIsEmpty()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request, recipeIllustration: null).ShouldThrowAsync<ErrorOnValidationException>();
        exception.ShouldSatisfyAllConditions(ex =>
        {
            ex.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.BadRequest);
            ex.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.Count.ShouldBe(1);
                messages.ShouldContain(ResourceMessagesException.VALIDATION_TITLE_REQUIRED);
            });
        });
    }

    private RecipeRegisterUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var recipeWriteOnlyRepository = new IRecipeWriteOnlyRepositoryBuilder().Build();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var storageService = IStorageServiceBuilder.Build();

        return new RecipeRegisterUseCase(recipeWriteOnlyRepository, loggedUser, unitOfWork, storageService);
    }
}