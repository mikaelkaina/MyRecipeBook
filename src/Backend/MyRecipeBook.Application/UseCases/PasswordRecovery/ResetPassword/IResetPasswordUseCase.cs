using MyRecipeBook.Communication.Requets.VerificationCode;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;

public interface IResetPasswordUseCase
{
    Task Execute(RequestResetPasswordJson request);
}
