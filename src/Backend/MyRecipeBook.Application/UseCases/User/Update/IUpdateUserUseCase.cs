using MyRecipeBook.Communication.Requets.User;

namespace MyRecipeBook.Application.UseCases.User.Update;

public interface IUpdateUserUseCase
{
    Task Execute(RequestUpdateUserJson request);
}
