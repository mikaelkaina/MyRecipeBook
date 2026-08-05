using MyRecipeBook.Communication.Requets.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.UpdateById;

public interface IRecipeUpdateByIdUseCase
{
    Task Execute(Guid recipeId, RequestRecipeJson request);
}
