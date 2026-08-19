using Bogus;
using MyRecipeBook.Communication.Requets.User;

namespace CommonTestsUtilities.Requests.Member;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int newPasswordLength = 10)
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(request => request.CurrentPassword, f => f.Internet.Password())
            .RuleFor(request => request.NewPassword, f => f.Internet.Password(length: newPasswordLength));
    }
}
