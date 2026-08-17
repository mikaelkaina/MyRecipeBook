using MyRecipeBook.Communication.Requets.User;
using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.Application.UseCases.User.Register;

public interface IRegisterUserAccountUseCase
{
    Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserAccountJson request);
}
