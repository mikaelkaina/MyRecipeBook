using FluentValidation;
using MyRecipeBook.Communication.Requets;

namespace MyRecipeBook.Application.UseCases.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        
    }
}
