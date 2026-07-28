using MyRecipeBook.Communication.Requets;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;

public interface IRecipeRegisterUseCase
{
    Task<ResponseRegiteredRecipeJson> Execute(RequestRecipeJson request);
}