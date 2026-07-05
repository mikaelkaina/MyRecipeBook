using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Infrastructure.Security.PasswordHashing;

namespace CommonTestsUtilities.Entities;

public class UserBuilder
{
    public static (User user, string plainPassword) Build()
    {
        var plainPassword = new Faker().Internet.Password();
        var passwordHasher = new Argon2PasswordHasher();

        var user = new Faker<User>()
            .RuleFor(u => u.Name, f => f.Person.FirstName)
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
            .RuleFor(u => u.Password, _ => passwordHasher.HashPassword(plainPassword));

        return (user, plainPassword);
    }
}
