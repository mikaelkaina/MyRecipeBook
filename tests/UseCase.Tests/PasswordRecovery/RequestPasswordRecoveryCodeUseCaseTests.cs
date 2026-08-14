using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Repositories;
using CommonTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;
using Shouldly;

namespace UseCase.Tests.PasswordRecovery;

public class RequestPasswordRecoveryCodeUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var request = RequestPasswordRecoveryJsonBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Success_ShouldDoNothing_WhenEmailNotFound()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();

        var useCase = CreateUseCase();

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    private RequestPasswordRecoveryCodeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User? user = null)
    {
        var userReadOnlyRepository = new IUserReadOnlyRepositoryBuilder();

        if (user is not null)
            userReadOnlyRepository.GetByEmail(user);

        var verificationCodeRepository = IVerificationCodeWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        return new RequestPasswordRecoveryCodeUseCase(
            userReadOnlyRepository.Build(),
            verificationCodeRepository,
            unitOfWork);
    }
}
