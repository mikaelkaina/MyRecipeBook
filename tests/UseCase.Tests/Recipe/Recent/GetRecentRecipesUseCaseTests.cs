using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories.Formula;
using CommonTestsUtilities.Storage;
using MyRecipeBook.Application.UseCases.Recipe.Recent;
using Shouldly;

namespace UseCase.Tests.Recipe.Recent;

public class GetRecentRecipesUseCaseTests
{
    [Theory]
    [InlineData(true, IStorageServiceBuilder.FakeUrl)]
    [InlineData(false, "")]
    public async Task Success(bool hasImage, string expectedUrl)
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        recipe.HasImage = hasImage;

        var useCase = CreateUseCase(user, [recipe]);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.Count.ShouldBe(1);
        result.Recipes.ShouldContain(recipeSummary => recipeSummary.Id == recipe.Id && recipeSummary.Title.Equals(recipe.Title));
        result.Recipes.ShouldAllBe(recipeSummary => recipeSummary.ImageUrl.Equals(expectedUrl));
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

        var storageService = IStorageServiceBuilder.Build();

        return new GetRecentRecipesUseCase(loggedUser, repository, storageService);
    }
}