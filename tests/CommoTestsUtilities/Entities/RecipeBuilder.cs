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

    public static List<Recipe> BuildMany(User user, int count = 2)
    {
        var instructionOrder = 1;

        return Enumerable.Range(0, count).Select((_, index) =>
            new Faker<Recipe>()
                .RuleFor(r => r.Title, f => f.Lorem.Word())
                .RuleFor(r => r.CookTime, f => f.PickRandom<CookTime>())
                .RuleFor(r => r.UserId, _ => user.Id)
                .RuleFor(r => r.Active, _ => true)
                .RuleFor(r => r.CreatedAt, _ => DateTime.UtcNow.AddDays(-index))
                .RuleFor(r => r.DishTypes, f => f.Make(1, () => new RecipeDishType
                {
                    Type = f.PickRandom<DishType>()
                }).ToList())
                .RuleFor(r => r.Ingredients, f => f.Make(3, () => new RecipeIngredient
                {
                    Item = f.Commerce.ProductName()
                }).ToList())
                .RuleFor(r => r.Instructions, f => f.Make(3, () => new RecipeInstruction
                {
                    Order = instructionOrder++,
                    Description = f.Lorem.Sentence()
                }).ToList())
                .Generate()
        ).ToList();
    }
}