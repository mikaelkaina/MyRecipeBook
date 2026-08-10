using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.Recent;
using Shouldly;

namespace UseCase.Tests.Recipe.Recent;

public class GetRecentRecipesUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.BuildMany(user, count: 2);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeEmpty();
        result.Recipes.Count.ShouldBe(2);
        result.Recipes.ShouldAllBe(recipe => recipes.Any(r => r.Id == recipe.Id && r.Title == recipe.Title));
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenNoRecipesExist()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user, []);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.ShouldBeEmpty();
    }

    private GetRecentRecipesUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        List<MyRecipeBook.Domain.Entities.Recipe> recipes)
    {
        var repository = new IRecipeReadOnlyRepositoryBuilder()
            .GetRecentRecipes(user, recipes)
            .Build();

        var loggedUser = ILoggedUserBuilder.Build(user);

        return new GetRecentRecipesUseCase(loggedUser, repository);
    }
}