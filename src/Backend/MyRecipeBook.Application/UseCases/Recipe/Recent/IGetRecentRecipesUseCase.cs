using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Recent;

public interface IGetRecentRecipesUseCase
{
    Task<ResponseRecipesJson> Execute();
}