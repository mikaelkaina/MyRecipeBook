using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Tests.Recipe.GetById;

public class GetRecipeByIdUseCaseTests
{
    static GetRecipeByIdUseCaseTests()
    {
        MapsterConfiguration.Configure();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, recipe);

        var result = await useCase.Execute(recipe.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(recipe.Id);
        result.Title.ShouldBe(recipe.Title);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenRecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(Guid.NewGuid()).ShouldThrowAsync<NotFoundException>();
        exception.ShouldSatisfyAllConditions(ex =>
        {
            ex.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.NotFound);
            ex.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.Count.ShouldBe(1);
                messages.ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
            });
        });
    }

    private GetRecipeByIdUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var repositoryBuilder = new IRecipeReadOnlyRepositoryBuilder();

        if (recipe is not null)
            repositoryBuilder.GetById(user, recipe);

        var loggedUser = ILoggedUserBuilder.Build(user);

        return new GetRecipeByIdUseCase(repositoryBuilder.Build(), loggedUser);
    }
}