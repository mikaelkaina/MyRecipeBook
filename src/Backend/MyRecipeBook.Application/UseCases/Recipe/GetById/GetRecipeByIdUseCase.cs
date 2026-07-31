using Mapster;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public class GetRecipeByIdUseCase : IGetRecipeByIdUseCase
{
    public async Task<ResponseRecipeJson> Execute(Guid recipeId)
    {
        var recipe = new Domain.Entities.Recipe();
        if (recipe is null)
            throw new NotFoundException(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);

        return recipe.Adapt<ResponseRecipeJson>();
    }
}
