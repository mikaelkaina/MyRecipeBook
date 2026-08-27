using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public interface IFilterRecipesUseCase
{
    Task<ResponseRecipesJson> Execute(RequestFilterRecipesJson? request);
}