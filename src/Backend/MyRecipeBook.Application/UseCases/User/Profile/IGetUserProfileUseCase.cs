using MyRecipeBook.Communication.Responses.User;

namespace MyRecipeBook.Application.UseCases.User.Profile;

public interface IGetUserProfileUseCase
{
    Task<ResponseUserProfileJson> Execute();
}
