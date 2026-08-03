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
            .RuleFor(r => r.Id, _ => Guid.CreateVersion7())
            .RuleFor(r => r.Title, f => f.Lorem.Word())
            .RuleFor(r => r.CookTime, f => f.PickRandom<CookTime>())
            .RuleFor(r => r.UserId, _ => user.Id)
            .RuleFor(r => r.Active, _ => true)
            .RuleFor(r => r.DishTypes, f => f.Make(1, () => new RecipeDishType
            {
                Type = f.PickRandom<DishType>()
            }).ToList())
            .RuleFor(r => r.Ingredients, f => f.Make(3, () => new RecipeIngredient
            {
                Item = f.Commerce.ProductName()
            }).ToList())
            .RuleFor(r => r.Instructions, f => [.. f.Make(3, () => new RecipeInstruction
            {
                Order = instructionOrder++,
                Description = f.Lorem.Sentence()
            })]);
    }
}