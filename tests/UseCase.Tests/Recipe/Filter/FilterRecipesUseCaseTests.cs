using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories.Formula;
using CommonTestsUtilities.Storage;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Communication.Requets.Recipe;
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

    [Theory]
    [InlineData(true, IStorageServiceBuilder.FakeUrl)]
    [InlineData(false, "")]
    public async Task Success(bool hasImage, string expectedUrl)
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        recipe.HasImage = hasImage;

        var useCase = CreateUseCase(user, [recipe]);

        var request = new RequestFilterRecipesJson();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.Count.ShouldBe(1);
        result.Recipes.ShouldContain(recipeSummary => recipeSummary.Id == recipe.Id && recipeSummary.Title.Equals(recipe.Title));
        result.Recipes.ShouldAllBe(recipeSummary => recipeSummary.ImageUrl.Equals(expectedUrl));
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