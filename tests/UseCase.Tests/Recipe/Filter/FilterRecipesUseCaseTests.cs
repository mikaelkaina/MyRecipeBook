using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories.Formula;
using CommonTestsUtilities.Requests.Recipe;
using CommonTestsUtilities.Storage;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using Shouldly;

namespace UseCase.Tests.Recipe.Filter;

public class FilterRecipesUseCaseTests
{
    [Theory]
    [InlineData(true, IStorageServiceBuilder.FakeUrl)]
    [InlineData(false, "")]
    public async Task Success_WhenRequestIsNull(bool hasImage, string expectedUrl)
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        recipe.HasImage = hasImage;

        var useCase = CreateUseCase(user, [recipe]);

        var result = await useCase.Execute(request: null);

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.Count.ShouldBe(1);
        result.Recipes.ShouldContain(recipeSummary => recipeSummary.Id == recipe.Id && recipeSummary.Title.Equals(recipe.Title));
        result.Recipes.ShouldAllBe(recipeSummary => recipeSummary.ImageUrl.Equals(expectedUrl));
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

        var storageService = IStorageServiceBuilder.Build();

        return new FilterRecipesUseCase(repository, loggedUser, storageService);
    }
}