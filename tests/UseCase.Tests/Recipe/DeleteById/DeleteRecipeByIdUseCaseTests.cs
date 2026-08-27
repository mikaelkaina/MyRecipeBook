using CommonTestsUtilities.Entities;
using CommonTestsUtilities.Identity;
using CommonTestsUtilities.Repositories.Formula;
using CommonTestsUtilities.Storage;
using MyRecipeBook.Application.UseCases.Recipe.DeleteById;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Tests.Recipe.DeleteById;

public class DeleteRecipeByIdUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(recipe, user);

        await useCase.Execute(recipe.Id).ShouldNotThrowAsync();
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

    private DeleteRecipeByIdUseCase CreateUseCase(MyRecipeBook.Domain.Entities.Recipe recipe,
        MyRecipeBook.Domain.Entities.User user)
    {
        var repository = new IRecipeWriteOnlyRepositoryBuilder().DeleteById(recipe).Build();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var storageService = IStorageServiceBuilder.Build();

        return new DeleteRecipeByIdUseCase(repository, loggedUser, storageService);
    }
}