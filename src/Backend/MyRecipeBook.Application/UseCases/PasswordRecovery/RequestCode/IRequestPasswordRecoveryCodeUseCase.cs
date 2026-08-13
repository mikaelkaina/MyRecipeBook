using MyRecipeBook.Communication.Requets.VerificationCode;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;

public interface IRequestPasswordRecoveryCodeUseCase
{
    Task Execute(RequestPasswordRecoveryJson request);
}
