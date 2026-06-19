using MyRecipeBook.Communication.Requets;

namespace MyRecipeBook.Application.UseCases.User.Register;

public interface IRegisterUserAccountUseCase
{
    void Execute(RequestRegisterUserAccountJson request);
}
