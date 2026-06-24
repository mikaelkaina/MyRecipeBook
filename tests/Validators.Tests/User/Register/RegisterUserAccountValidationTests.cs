using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Communication.Requets;

namespace Validators.Tests.User.Register;

public class RegisterUserAccountValidationTests
{
    [Fact]
    public void Sucess()
    {
        var request = new RequestRegisterUserAccountJson
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
