using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Infrastructure.Security.Tokens.Access;

namespace CommonTestsUtilities.Secutiry;

public class JwtTokenBuilder
{
    public static string Build(User user, string signingKey, uint expirationTimeMinutes)
    {
        var handler = new JwtTokenHandler(expirationTimeMinutes, signingKey);
        return handler.Generate(user);
    }
}
