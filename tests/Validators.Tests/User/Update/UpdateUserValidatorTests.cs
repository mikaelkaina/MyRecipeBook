using CommonTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exception;
using Shouldly;

namespace Validators.Tests.User.Update;

public class UpdateUserValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        var validator = new UpdateUserValidator();

        var resquest = RequestUpdateUserJsonBuilder.Build();
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
        var resquest = RequestUpdateUserJsonBuilder.Build();
        resquest.Email = email!;

        var validator = new UpdateUserValidator();

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailIsInvalid()
    {
        var resquest = RequestUpdateUserJsonBuilder.Build();
        resquest.Email = "invalid-email";

        var validator = new UpdateUserValidator();

        var result = validator.Validate(resquest);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_INVALID));
        });
    }
}
