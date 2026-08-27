using Mapster;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.UpdateById;

public class RecipeUpdateByIdUseCase : IRecipeUpdateByIdUseCase
{
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public RecipeUpdateByIdUseCase(IRecipeUpdateOnlyRepository repository, 
        ILoggedUser loggedUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute(Guid recipeId, RequestRecipeJson request)
    {
        ValidateAndThrowOnFailures(request);

        var recipe = await _repository.GetById(recipeId, _loggedUser.GetUserId());
        if(recipe is null)
            throw new NotFoundException(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);

        request.Adapt(recipe);

        await _unitOfWork.Commit();
    }

    private void ValidateAndThrowOnFailures(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);
        if (result.IsValid == false)
            throw new ErrorOnValidationException([.. result.Errors.Select(error => error.ErrorMessage)]);
    }
}
