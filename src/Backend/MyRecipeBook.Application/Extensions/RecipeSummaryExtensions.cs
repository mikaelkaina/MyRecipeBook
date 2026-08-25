using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Storage;

namespace MyRecipeBook.Application.Extensions;

internal static class RecipeSummaryExtensions
{
    extension(IList<RecipeSummaryDto> recipes)
    {
        internal IList<ResponseRecipeSummaryJson> ToResponseJson(Guid userId, IStorageService storageService)
        {
            return recipes
                .Select(recipe => new ResponseRecipeSummaryJson
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    ImageUrl = recipe.HasImage ? storageService.GetRecipeIllustrationUrl(userId: userId, recipeId: recipe.Id) : string.Empty
                }).ToList();
        }
    }
}
