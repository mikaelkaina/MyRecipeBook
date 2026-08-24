using Mapster;
using MyRecipeBook.Communication.Responses.User;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Storage;

namespace MyRecipeBook.Application.UseCases.User.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IStorageService _storageService;

    public GetUserProfileUseCase(ILoggedUser loggedUser, IStorageService storageService)
    {
        _loggedUser = loggedUser;
        _storageService = storageService;
    }

    public async Task<ResponseUserProfileJson> Execute()
    {
        var loggedUser = await _loggedUser.Get();

        var response = loggedUser.Adapt<ResponseUserProfileJson>();
        response.ImagemUrl = _storageService.GetProfilePictureUrl(loggedUser);

        return response;
    }
}
