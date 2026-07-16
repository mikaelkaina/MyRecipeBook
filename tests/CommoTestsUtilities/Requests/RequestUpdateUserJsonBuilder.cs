using Bogus;
using MyRecipeBook.Communication.Requets;

namespace CommonTestsUtilities.Requests;

public class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        return new Faker<RequestUpdateUserJson>()
            .RuleFor(request => request.Name, f => f.Person.FullName)
            .RuleFor(request => request.Email, f => f.Internet.Email());
    }
}
