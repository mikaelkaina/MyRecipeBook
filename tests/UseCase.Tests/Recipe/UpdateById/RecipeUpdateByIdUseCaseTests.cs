using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories;
using CommonTestsUtilities.Requests.Recipe;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Recipe.UpdateById;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;
using System.Net;

namespace UseCase.Tests.Recipe.UpdateById;

public class RecipeUpdateByIdUseCaseTests
{
    static RecipeUpdateByIdUseCaseTests()
    {
        MapsterConfiguration.Configure();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(recipe, user);

        await useCase.Execute(recipe.Id, request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenRecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(recipe, user);

        var exception = await useCase.Execute(Guid.CreateVersion7(), request).ShouldThrowAsync<NotFoundException>();
        exception.ShouldSatisfyAllConditions(ex =>
        {
            ex.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);
            ex.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.Count.ShouldBe(1);
                messages.ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
            });
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenTitleIsEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var useCase = CreateUseCase(recipe, user);

        var exception = await useCase.Execute(recipe.Id, request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.ShouldSatisfyAllConditions(ex =>
        {
            ex.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);
            ex.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.Count.ShouldBe(1);
                messages.ShouldContain(ResourceMessagesException.VALIDATION_TITLE_REQUIRED);
            });
        });
    }

    private RecipeUpdateByIdUseCase CreateUseCase(MyRecipeBook.Domain.Entities.Recipe recipe,
        MyRecipeBook.Domain.Entities.User user)
    {
        var repository = new IRecipeUpdateOnlyRepositoryBuilder().GetById(recipe).Build();

        var loggedUser = ILoggedUserBuilder.Build(user);
        var unitOfWork = IUnitOfWorkBuilder.Build();

        return new RecipeUpdateByIdUseCase(repository, loggedUser, unitOfWork);
    }
}