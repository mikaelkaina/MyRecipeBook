using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestsUtilities.Repositories;

public class IUnitOfWorkBuilder
{
    public static IUnitOfWork Build()
    {
        var moq = new Mock<IUnitOfWork>();

        return moq.Object;
    }
}
