using CommoTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidationTests
{
    [Fact]
    public void Sucess()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
