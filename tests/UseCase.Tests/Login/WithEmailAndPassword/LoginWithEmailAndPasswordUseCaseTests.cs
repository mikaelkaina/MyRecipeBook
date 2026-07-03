using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Repositories;
using CommonTestsUtilities.Requests;
using CommonTestsUtilities.Secutiry;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Tests.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(request.Password, user);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(user.Name);
        result.Tokens.AccessToken.ShouldBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldThrowException_WhenUserDoesExist()
    {
        var request = RequestLoginJsonBuilder.Build();

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<InvalidLoginException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMensages =>
        {
            errorMensages.Count.ShouldBe(1);
            errorMensages.ShouldContain(ResourceMessagesException.VALIDATION_LOGIN_INVALID);
        });
    }

    [Fact]
    public async Task ShouldThrowException_WhenPasswordIsIncorrect()
    {
        var user = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user: user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<InvalidLoginException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMensages =>
        {
            errorMensages.Count.ShouldBe(1);
            errorMensages.ShouldContain(ResourceMessagesException.VALIDATION_LOGIN_INVALID);
        });
    }

    private LoginWithEmailAndPasswordUseCase CreateUseCase(string? password = null, MyRecipeBook.Domain.Entities.User? user = null)
    {
        var passwordHasherBuilder = new IPasswordHasherBuilder();
        if(password.IsNotEmpty())
            passwordHasherBuilder.VerifyPassword(password);
        
         var userReadOnlyRepositoryBuilder = new IUserReadOnlyRepositoryBuilder();
        if(user is not null)
            userReadOnlyRepositoryBuilder.GetByEmail(user);

        return new LoginWithEmailAndPasswordUseCase(passwordHasherBuilder.Build(), userReadOnlyRepositoryBuilder.Build());
    }
}
