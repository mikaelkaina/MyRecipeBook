using Mapster;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Domain.Entities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UseCase.Tests")]
namespace MyRecipeBook.Application.Mappings;

internal static class MapsterConfiguration
{
    internal static void Configure()
    {
        TypeAdapterConfig<RequestRecipeJson, Recipe>
            .NewConfig()
            .Map(destination => destination.Ingredients, request => request.Ingredients.Select(ingredient =>
                new RecipeIngredient()
                {
                    Item = ingredient
                }))
            .Map(destination => destination.DishTypes, request => request.DishTypes.Select(dishType =>
                new RecipeDishType()
                {
                    Type = (Domain.Enums.DishType)dishType
                }));
    }
}