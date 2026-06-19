using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Application.UseCases.User.Register;

namespace MyRecipeBook.Application;

public class DependencyInjection
{
    public static void AddApplication(IServiceCollection services)
    {
        services.AddScoped<IRegisterUserAccountUseCase , RegisterUserAccountUseCase>();
    }
}
