using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public interface IFilterRecipesUseCase
{
    Task<ResponseRecipesJson> Excute(RequestFilterRecipesJson? request);
}