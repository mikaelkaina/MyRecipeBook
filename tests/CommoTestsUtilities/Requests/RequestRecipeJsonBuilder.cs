using Bogus;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requets;

namespace CommonTestsUtilities.Requests;

public class RequestRecipeJsonBuilder
{
    public static RequestRecipeJson Build()
    {
        var instructionOrder = 1;

        return new Faker<RequestRecipeJson>()
            .RuleFor(r => r.Title, f => f.Lorem.Word())
            .RuleFor(r => r.CookTime, f => f.PickRandom<CookTime>())
            .RuleFor(r => r.DishTypes, f => f.Make(1, () => f.PickRandom<DishType>()).ToList())
            .RuleFor(r => r.Ingredients, f => f.Make(3, () => f.Commerce.ProductName()).ToList())
            .RuleFor(r => r.Instructions, f => f.Make(3, () => new RequestRecipeInstructionJson
            {
                Order = instructionOrder++,
                Description = f.Lorem.Sentence()
            }).ToList());
    }
}
