using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories.Formula;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Tests.Recipe.GetById;

public class GetRecipeByIdUseCaseTests
{
    static GetRecipeByIdUseCaseTests()
    {
        MapsterConfiguration.Configure();
    }

    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(recipe, user);

        var result = await useCase.Execute(recipe.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(recipe.Id);
        result.Title.ShouldBe(recipe.Title);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenRecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(recipe, user);

        var exception = await useCase.Execute(Guid.CreateVersion7()).ShouldThrowAsync<NotFoundException>();
        exception.ShouldSatisfyAllConditions(ex =>
        {
            ex.GetStatusCode().ShouldBe(System.Net.HttpStatusCode.NotFound);
            ex.GetErrorMessages().ShouldSatisfyAllConditions(messages =>
            {
                messages.Count.ShouldBe(1);
                messages.ShouldContain(ResourceMessagesException.VALIDATION_RECIPE_NOT_FOUND);
            });
        });
    }

    private GetRecipeByIdUseCase CreateUseCase(MyRecipeBook.Domain.Entities.Recipe recipe,
        MyRecipeBook.Domain.Entities.User user)
    {
        var repository = new IRecipeReadOnlyRepositoryBuilder().GetById(recipe).Build();

        var loggedUser = ILoggedUserBuilder.Build(user);

        return new GetRecipeByIdUseCase(repository, loggedUser);
    }
}