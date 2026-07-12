using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Identity;

namespace CommonTestsUtilities.Identity;

public class ILoggedUderBuilder
{
    public static ILoggedUser Build(User user)
    {
        var mock = new Mock<ILoggedUser>();

        mock.Setup(loggedUser => loggedUser.Get()).ReturnsAsync(user);
        mock.Setup(loggedUser => loggedUser.GetUserId()).Returns(user.Id);

        return mock.Object;
    }

}
