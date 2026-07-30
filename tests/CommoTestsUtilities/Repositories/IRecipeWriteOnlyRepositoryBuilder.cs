using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestsUtilities.Repositories;

public class IRecipeWriteOnlyRepositoryBuilder
{
    public static IRecipeWriteOnlyRepository Build()
    {
        var mock = new Mock<IRecipeWriteOnlyRepository>();
        return mock.Object;
    }
}
