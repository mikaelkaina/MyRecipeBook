using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public interface IGetRecipeByIdUseCase
{
    Task<ResponseRecipeJson> Execute(Guid recipeId);
}
