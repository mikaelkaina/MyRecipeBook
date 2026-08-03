using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestsUtilities.Repositories;

public class IRecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _mock = new();

    public IRecipeReadOnlyRepositoryBuilder GetById(User user, Recipe recipe)
    {
        _mock.Setup(r => r.GetById(recipe.Id, user.Id)).ReturnsAsync(recipe);
        return this;
    }

    public IRecipeReadOnlyRepository Build() => _mock.Object;
}