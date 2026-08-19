using Bogus;
using MyRecipeBook.Communication.Requets.VerificationCode;

namespace CommonTestsUtilities.Requests.Member;

public class RequestPasswordRecoveryJsonBuilder
{
    public static RequestPasswordRecoveryJson Build()
    {
        return new Faker<RequestPasswordRecoveryJson>()
            .RuleFor(r => r.Email, f => f.Internet.Email());
    }
}


