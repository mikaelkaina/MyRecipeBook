using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Files;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories.Member;
using CommonTestsUtilities.Storage;
using MyRecipeBook.Application.UseCases.User.ChangeProfilePicture;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;
using System.Net;

namespace UseCase.Tests.User.ChangeProfilePicture;

public class ChangeProfilePictureUseCaseTests
{
    [Fact]
    public async Task Success_WhenPng()
    {
        var useCase = CreateUseCase();

        await useCase.Execute(FileBuilder.GetPng()).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Success_WhenJpeg()
    {
        var useCase = CreateUseCase();

        await useCase.Execute(FileBuilder.GetJpeg()).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_WhenImageIsTxt()
    {
        var useCase = CreateUseCase();

        var exception = await useCase.Execute(FileBuilder.GetTxt()).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.BadRequest);

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesException.VALIDATON_ONLY_IMAGES_ACCEPTED);
        });
    }

    private static ChangeProfilePictureUseCase CreateUseCase()
    {
        var (user, _) = UserBuilder.Build();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var storageService = IStorageServiceBuilder.Build();
        var userUpdateOnlyRepository = IUserUpdateOnlyRepositoryBuilder.Build();

        return new ChangeProfilePictureUseCase(loggedUser, storageService, userUpdateOnlyRepository);
    }
}
