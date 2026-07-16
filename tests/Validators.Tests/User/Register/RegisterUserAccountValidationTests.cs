using CommonTestsUtilities.Requests;
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        var validator = new RegisterUserAccountValidation();

        var resquest = RequestRegisterUserAccountJsonBuilder.Build();
        resquest.Name = name!;

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_NAME_REQUIRED));
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty(string? email)
    {
        var resquest = RequestRegisterUserAccountJsonBuilder.Build();
        resquest.Email = email!;

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
    public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
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
    public void Validate_ShouldHaveError_WhenEmailIsInvalid()
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

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Validate_ShouldHaveError_WhenPasswordIsInvalid(int passwordLength)
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build(passwordLength);

        var validator = new RegisterUserAccountValidation();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_MIN_LENGTH));
        });
    }
}
