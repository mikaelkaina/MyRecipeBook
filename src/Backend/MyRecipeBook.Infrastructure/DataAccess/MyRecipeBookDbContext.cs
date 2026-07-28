using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;

[assembly: InternalsVisibleTo("WebApi.Tests")]
namespace MyRecipeBook.Infrastructure.DataAccess;

internal class MyRecipeBookDbContext : DbContext
{
    public MyRecipeBookDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

    public DbSet<User> Users { get; set; }
    
    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyRecipeBookDbContext).Assembly);

        modelBuilder.Entity<RecipeDishType>().ToTable("RecipeDishTypes");
        modelBuilder.Entity<RecipeIngredient>().ToTable("RecipeIngredients");
        modelBuilder.Entity<RecipeInstruction>().ToTable("RecipeInstructions");
    }
}
