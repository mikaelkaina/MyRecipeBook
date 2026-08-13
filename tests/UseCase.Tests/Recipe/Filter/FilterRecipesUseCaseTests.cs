using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using Shouldly;

namespace UseCase.Tests.Recipe.Filter;

public class FilterRecipesUseCaseTests
{
    static FilterRecipesUseCaseTests()
    {
        MapsterConfiguration.Configure();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.BuildMany(user, count: 3);
        var request = RequestFilterRecipesJsonBuilder.Build();

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Excute(request);

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeEmpty();
        result.Recipes.Count.ShouldBe(3);
        result.Recipes.ShouldAllBe(r => recipes.Any(recipe =>
            recipe.Id == r.Id && recipe.Title == r.Title));
    }

    [Fact]
    public async Task Success_ShouldReturnEmptyList_WhenNoRecipesMatchFilter()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestFilterRecipesJsonBuilder.Build();

        var useCase = CreateUseCase(user, []);

        var result = await useCase.Excute(request);

        result.ShouldNotBeNull();
        result.Recipes.ShouldBeEmpty();
    }

    [Fact]
    public async Task Success_ShouldReturnAllRecipes_WhenRequestIsNull()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = RecipeBuilder.BuildMany(user, count: 3);

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Excute(null);

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeEmpty();
        result.Recipes.Count.ShouldBe(3);
    }

    private FilterRecipesUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        List<MyRecipeBook.Domain.Entities.Recipe> recipes)
    {
        var repository = new IRecipeReadOnlyRepositoryBuilder()
            .FilterRecipes(user, recipes)
            .Build();

        var loggedUser = ILoggedUserBuilder.Build(user);

        return new FilterRecipesUseCase(repository, loggedUser);
    }
}