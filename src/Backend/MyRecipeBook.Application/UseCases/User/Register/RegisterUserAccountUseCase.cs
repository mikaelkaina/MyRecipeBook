using Mapster;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserAccountUseCase : IRegisterUserAccountUseCase
{
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserAccountUseCase(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public void Execute(RequestRegisterUserAccountJson request)
    {
        ValidateAndThrowOnFailures(request);

        var user = request.Adapt<Domain.Entities.User>();

        user.Password = _passwordHasher.HashPassword(request.Password);
    }

    private void ValidateAndThrowOnFailures(RequestRegisterUserAccountJson request)
    {
        var validation = new RegisterUserAccountValidation();

        var result = validation.Validate(request);

        if (result.IsValid == false)
        {
            var errorMensages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMensages);
        }
    }
}
