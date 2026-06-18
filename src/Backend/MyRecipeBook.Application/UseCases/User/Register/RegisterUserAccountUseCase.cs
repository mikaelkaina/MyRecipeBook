using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserAccountUseCase
{
    public void Execute(RequestRegisterUserAccountJson request)
    {
        var validation = new RegisterUserAccountValidation();

        var result = validation.Validate(request);

        if (result.IsValid == false)
        {
            var errorMensages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ArgumentException("errorMensages");
        }
    }
}
