using Mapster;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exception;
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
    
    public async Task<ResponseRegiteredRecipeJson> Execute(RequestRecipeJson request, Stream? recipeIllustration)
    {
        ValidateAndThrowOnFailures(request);
        
        var recipe = request.Adapt<Domain.Entities.Recipe>();
        recipe.UserId = _loggedUser.GetUserId();

        if(recipeIllustration is not null)
        {
            var contentType = recipeIllustration.DetectImageContentType();
            if(contentType.IsEmpty())
                throw new ErrorOnValidationException([ResourceMessagesException.VALIDATON_ONLY_IMAGES_ACCESS]);
        }
        
        await _recipeWriteOnlyRepository.Add(recipe);
        
        await _unitOfWork.Commit();
        
        return new ResponseRegiteredRecipeJson
        {
            Id = recipe.Id,
            Title = recipe.Title,
        };
    }

    private static void ValidateAndThrowOnFailures(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid == false)
            throw new ErrorOnValidationException([.. result.Errors.Select(error => error.ErrorMessage)]);
    }
}