using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Infrastructure.DataAcess;

namespace WebApi.Tests.Resource;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    internal async Task<MyRecipeBookDbContext> GetDbContext()
    {
        var scope = Services.CreateAsyncScope();
        return scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();
    }

    internal string GetJwtSigningKey()
    {
        var configuration = Services.GetRequiredService<IConfiguration>();
        return configuration.GetValue<string>("Jwt:SigningKey")!;
    }

    internal uint GetJwtExpirationTimeMinutes()
    {
        var configuration = Services.GetRequiredService<IConfiguration>();
        return configuration.GetValue<uint>("Jwt:ExpirationTimeMinutes")!;
    }
}