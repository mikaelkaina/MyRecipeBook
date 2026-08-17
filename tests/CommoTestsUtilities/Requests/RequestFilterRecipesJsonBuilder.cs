using Bogus;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Enums;

namespace CommonTestsUtilities.Requests;

public class RequestFilterRecipesJsonBuilder
{
    public static RequestFilterRecipesJson Build()
    {
        return new Faker<RequestFilterRecipesJson>()
            .RuleFor(r => r.SearchTerm, f => f.Lorem.Word())
            .RuleFor(r => r.CookTime, f => f.PickRandom<CookTime>())
            .RuleFor(r => r.DishTypes, f => f.Make(1, () => f.PickRandom<DishType>()).ToList());
    }
}
