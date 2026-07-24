using CommonTestsUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Exception;
using Shouldly;

namespace Validators.Tests.User.Recipe;

public class RecipeValidatorTests
{
    [Fact]
    public void Success()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string? title)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = title!;

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_TITLE_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = new string('a', 251);

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_TITLE_MAX_LENGTH));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCookTimeIsInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.CookTime = (CookTime)99;

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_COOK_TIME_INVALID));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDishTypesIsEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes = [];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_AT_LESAT_ONE_DISH_TYPE));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDishTypeIsInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.DishTypes = [(DishType)99];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_DISH_TYPE_INVALID));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIngredientsIsEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients = [];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_AT_LEAST_ONE_INGREDIENT));
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public void Validate_ShouldHaveError_WhenIngredientIsEmpty(string ingredient)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients = [ingredient];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_INGREDIENT_EMPTY));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIngredientExceedsMaxLength()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Ingredients = [new string('a', 251)];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_INGREDIENT_MAX_LENGTH));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenInstructionsIsEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions = [];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_AT_LEAST_ONE_INSTRUCTION));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenInstructionOrderIsDuplicated()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 1, Description = "First step" },
            new RequestRecipeInstructionJson { Order = 1, Description = "Duplicate order" }
        ];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_INSTRUCTION_ORDER_DUPLICATED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenInstructionOrderIsInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 0, Description = "Some step" }
        ];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_INSTRUCTION_ORDER_INVALID));
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public void Validate_ShouldHaveError_WhenInstructionDescriptionIsEmpty(string description)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 1, Description = description }
        ];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_INSTRUCTION_DESCRIPTION_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenInstructionDescriptionExceedsMaxLength()
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Instructions =
        [
            new RequestRecipeInstructionJson { Order = 1, Description = new string('a', 2001) }
        ];

        var validator = new RecipeValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_INSTRUCTION_DESCRIPTION_MAX_LENGTH));
        });
    }
}