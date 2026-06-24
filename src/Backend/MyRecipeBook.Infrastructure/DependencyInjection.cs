using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Infrastructure.DataAcess;
using MyRecipeBook.Infrastructure.DataAcess.Repositories;
using MyRecipeBook.Infrastructure.Security.PasswordHashing;

namespace MyRecipeBook.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();

        services.AddDbContext<MyRecipeBookDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}