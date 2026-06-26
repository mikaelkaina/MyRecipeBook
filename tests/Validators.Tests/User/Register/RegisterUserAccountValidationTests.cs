using CommoTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exception;
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

    [Fact]
    public void Validate_SholdHaveError_WhenNameIsEmpty()
    {
        var resquest = RequestRegisterUserAccountJsonBuilder.Build();
        resquest.Name = string.Empty;

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_NAME_REQUIRED));
        });
    }

    [Fact]
    public void Validate_SholdHaveError_WhenEmailIsEmpty()
    {
        var resquest = RequestRegisterUserAccountJsonBuilder.Build();
        resquest.Email = string.Empty;

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_SholdHaveError_WhenPasswordIsEmpty()
    {
        var resquest = RequestRegisterUserAccountJsonBuilder.Build();
        resquest.Password = string.Empty;

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED));
        });
    }

    [Fact]
    public void Validate_SholdHaveError_WhenEmailIsInvalid()
    {
        var resquest = RequestRegisterUserAccountJsonBuilder.Build();
        resquest.Email = "invalid-email";

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_INVALID));
        });
    }
}
