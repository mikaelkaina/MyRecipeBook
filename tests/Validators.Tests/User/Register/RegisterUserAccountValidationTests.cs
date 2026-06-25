using CommoTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using Shouldly;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidationTests
{
    [Fact]
    public void Sucess()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }
}
