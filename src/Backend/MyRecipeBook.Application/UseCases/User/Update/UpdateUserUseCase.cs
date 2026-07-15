using FluentValidation.Results;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(IUserReadOnlyRepository userReadOnlyRepository,
        ILoggedUser loggedUser, IUserUpdateOnlyRepository userUpdateOnlyRepository  , IUnitOfWork unitOfWork)
    {
        _userReadOnlyRepository = userReadOnlyRepository;
        _loggedUser = loggedUser;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.Get();

        await Validate(request, loggedUser);

        loggedUser.Name = request.Name;
        loggedUser.Email = request.Email;

        _userUpdateOnlyRepository.UpdateProfile(loggedUser);

        await _unitOfWork.Commit();
    }

    private async Task Validate(RequestUpdateUserJson request, Domain.Entities.User loggedUser)
    {
        var validator = new UpdateUserValidator();

        var result = validator.Validate(request);

        if (loggedUser.Email.Equals(request.Email) == false)
        {
            var userExists = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if(userExists)
                result.Errors.Add(new ValidationFailure("email", ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS));
        }

        if(result.IsValid == false)
        {
            var errorMenssages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMenssages);
        }
    }
}   