using FluentValidation;
using MyRecipeBook.Communication.Requets;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserAccountValidation : AbstractValidator<RequestRegisterUserAccountJson>
{
    public RegisterUserAccountValidation()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required.");
        RuleFor(user => user.Password).NotEmpty().WithMessage("Password is required.");
        When(user => string.IsNullOrEmpty(user.Email), () =>
        {
            RuleFor(user => user.Email).EmailAddress().WithMessage("Invalid email format.");
        });
    }
}
