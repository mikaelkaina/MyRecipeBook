using FluentValidation;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Exception;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserAccountValidation : AbstractValidator<RequestRegisterUserAccountJson>
{
    public RegisterUserAccountValidation()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_NAME_REQUIRED);
        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED);
        RuleFor(user => user.Password).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED);
        When(user => string.IsNullOrEmpty(user.Email), () =>
        {
            RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_INVALID);
        });
    }
}
