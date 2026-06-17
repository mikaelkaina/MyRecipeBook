using MyRecipeBook.Communication.Requets;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserAccountUseCase
{
    public void Execute(RequestRegisterUserAccountJson request)
    {
        var validation = new RegisterUserAccountValidation();

        var result = validation.Validate(request);
    }
}
