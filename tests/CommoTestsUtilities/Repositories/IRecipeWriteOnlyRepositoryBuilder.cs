using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestsUtilities.Repositories;

public class IRecipeWriteOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeWriteOnlyRepository> _mock = new();

    public IRecipeWriteOnlyRepositoryBuilder DeleteById(Recipe recipe)
    {
        _mock.Setup(r => r.DeleteById(recipe.Id, recipe.UserId)).ReturnsAsync(true);
        return this;
    }

    public IRecipeWriteOnlyRepository Build() => _mock.Object;
}
