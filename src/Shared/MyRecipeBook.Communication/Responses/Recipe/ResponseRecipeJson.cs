using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Responses.Recipe;

public class ResponseRecipeJson
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public CookTime CookTime { get; set; }
    public IList<string> Ingredients { get; set; } = [];
    public ICollection<ResponseRecipeInstructionJson> Instructions { get; set; } = [];
    public ICollection<DishType> DishTypes { get; set; } = [];
}
