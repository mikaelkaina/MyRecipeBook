namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    Task<Entities.Recipe?> GetById(Guid recipeId, Guid userId);
    Task<List<Entities.Recipe>> GetRecentRecipes(Guid userId);
}
