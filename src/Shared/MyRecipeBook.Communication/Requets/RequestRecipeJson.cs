using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requets;

public class RequestRecipeJson
{
    public string Title { get; set; } = string.Empty;
    public CookTime CookTime { get; set; }
    public IList<string> Ingredients { get; set; } = [];
    public ICollection<RequestRecipeInstructionJson> Instructions { get; set; } = [];
    public ICollection<DishType> DishTypes { get; set; } = [];
}
