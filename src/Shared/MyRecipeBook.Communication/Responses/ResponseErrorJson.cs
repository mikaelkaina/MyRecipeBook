namespace MyRecipeBook.Communication.Responses;

public class ResponseErrorJson
{
    public List<string> Errors { get; private set; }
    public bool AccessTokenExpired { get; private set; }

    public ResponseErrorJson(List<string> errorMenssages) => Errors = errorMenssages;

    public ResponseErrorJson(string errorMessage) => Errors = [errorMessage];

    public ResponseErrorJson(string errorMenssage, bool accessTokenExpired)
    {
        Errors = [errorMenssage];
        AccessTokenExpired = accessTokenExpired;
    }
}
