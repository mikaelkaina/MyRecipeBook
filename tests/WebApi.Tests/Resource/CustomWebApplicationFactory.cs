using Microsoft.AspNetCore.Mvc.Testing;
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
}