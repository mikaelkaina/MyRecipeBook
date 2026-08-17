namespace MyRecipeBook.Communication.Responses.Recipe;

public class ResponseRecipesJson
{
    public IList<ResponseRecipeSummaryJson> Recipes { get; set; } = [];
}