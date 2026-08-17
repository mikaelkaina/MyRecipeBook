using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;

public interface IRecipeRegisterUseCase
{
    Task<ResponseRegiteredRecipeJson> Execute(RequestRecipeJson request);
}