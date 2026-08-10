using Mapster;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipesUseCase : IFilterRecipesUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;

    public FilterRecipesUseCase(IRecipeReadOnlyRepository recipeReadOnlyRepository,  ILoggedUser loggedUser)
    {
        _recipeReadOnlyRepository = recipeReadOnlyRepository;
        _loggedUser = loggedUser;
    }
    
    public async Task<ResponseRecipesJson> Excute(RequestFilterRecipesJson? request)
    {
        var recipes = await _recipeReadOnlyRepository.GetById(_loggedUser.GetUserId());
        
        return new ResponseRecipesJson()
        {
            Recipes = recipes.Adapt<IList<ResponseRecipeSummaryJson>>()
        };
    }
}