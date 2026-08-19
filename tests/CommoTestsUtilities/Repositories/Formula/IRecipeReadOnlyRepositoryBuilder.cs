using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestsUtilities.Repositories.Formula;

public class IRecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _mock = new();

    public IRecipeReadOnlyRepositoryBuilder GetById(Recipe recipe)
    {
        _mock.Setup(repository => repository.GetById(recipe.Id, recipe.UserId)).ReturnsAsync(recipe);
        return this;
    }

    public IRecipeReadOnlyRepositoryBuilder GetRecentRecipes(User user, List<Recipe> recipes)
    {
        var recipesDto = recipes.Select(recipe => new RecipeSummaryDto(recipe.Id, recipe.Title)).ToList();
        
        _mock.Setup(r => r.GetRecentRecipes(user.Id)).ReturnsAsync(recipesDto);
        return this;
    }

    public IRecipeReadOnlyRepositoryBuilder FilterRecipes(User user, List<Recipe> recipes)
    {
        _mock.Setup(r => r.FilterRecipes(user.Id, It.IsAny<RecipeFilterDto>())).ReturnsAsync(recipes);
        return this;
    }

    public IRecipeReadOnlyRepository Build() => _mock.Object;
}