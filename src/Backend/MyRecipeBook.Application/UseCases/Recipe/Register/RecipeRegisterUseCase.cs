using Mapster;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;

public class RecipeRegisterUseCase : IRecipeRegisterUseCase
{
    private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public RecipeRegisterUseCase(IRecipeWriteOnlyRepository recipeWriteOnlyRepository,
        ILoggedUser loggedUser, IUnitOfWork unitOfWork)
    {
        _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }
    
    public async  Task<ResponseRegiteredRecipeJson> Execute(RequestRecipeJson request)
    {
        Validate(request);
        
        var recipe = request.Adapt<Domain.Entities.Recipe>();
        recipe.UserId = _loggedUser.GetUserId();
        
        await _recipeWriteOnlyRepository.Add(recipe);
        
        await _unitOfWork.Commit();
        
        return new ResponseRegiteredRecipeJson
        {
            Id = recipe.Id,
            Title = recipe.Title,
        };
    }

    private static void Validate(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid == false)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }
}