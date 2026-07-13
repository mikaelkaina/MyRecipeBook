using MyRecipeBook.Communication.Requets;

namespace MyRecipeBook.Application.UseCases.ChangePassword;

public interface IChangePasswordUseCase
{
    Task Execute(RequestChangePasswordJson request);
}
