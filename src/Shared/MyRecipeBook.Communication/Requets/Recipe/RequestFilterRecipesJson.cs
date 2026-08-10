using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requets.Recipe;

public class RequestFilterRecipesJson
{
    public string? SearchTerm { get; set; }
    public CookTime? CookTime { get; set; }
    public IList<DishType> DishTypes { get; set; } = [];
}