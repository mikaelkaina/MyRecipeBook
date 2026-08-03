using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommonTestsUtilities.Entities;

public class RecipeBuilder
{
    public static Recipe Build(User user)
    {
        var instructionOrder = 1;

        return new Faker<Recipe>()
            .RuleFor(request => request.Title, f => f.Lorem.Word())
            .RuleFor(request => request.CookTime, f => f.PickRandom<CookTime>())
            .RuleFor(request => request.Ingredients, f => f.Make(3, () => new RecipeIngredient
            {
                Item = f.Commerce.ProductName()
            }))
            .RuleFor(request => request.DishTypes, f => f.Make(2, () => new RecipeDishType
            {
                Type = f.PickRandom<DishType>()
            }))
            .RuleFor(request => request.Instructions, f => f.Make(3, () => new RecipeInstruction
            {
                Order = instructionOrder++,
                Description = f.Lorem.Sentence()
            }))
            .RuleFor(request => request.UserId, f => user.Id);
    }
}