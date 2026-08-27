using Mapster;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Storage;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class FilterRecipesUseCase : IFilterRecipesUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _repository;
    private readonly IStorageService _storageService;

    public FilterRecipesUseCase(IRecipeReadOnlyRepository recipeReadOnlyRepository, 
        ILoggedUser loggedUser, IStorageService storageService)
    {
        _repository = recipeReadOnlyRepository;
        _loggedUser = loggedUser;
        _storageService = storageService;
    }
    
    public async Task<ResponseRecipesJson> Execute(RequestFilterRecipesJson? request)
    {
        var filter = request is null ? new RecipeFilterDto() : 
            new RecipeFilterDto
            {
                SearchTerm = request.SearchTerm,
                CookTime = (Domain.Enums.CookTime?)request.CookTime,
                DishTypes = request.DishTypes.Select(dishType => (Domain.Enums.DishType)dishType).ToList()
            };
        
        var recipes = await _repository.FilterRecipes(_loggedUser.GetUserId(), filter);
        
        return new ResponseRecipesJson()
        {
            Recipes = recipes.ToResponseJson(_loggedUser.GetUserId(), _storageService)
        };
    }
}