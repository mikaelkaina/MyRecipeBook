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

        modelBuilder.Entity<RecipeIngredient>().ToTable("RecipeIngredients")
            .Property(ingredient => ingredient.Id).ValueGeneratedNever();

        modelBuilder.Entity<RecipeInstruction>().ToTable("RecipeInstructions")
            .Property(instruction => instruction.Id).ValueGeneratedNever();

        modelBuilder.Entity<RecipeDishType>().ToTable("RecipeDishTypes")
            .Property(dishType => dishType.Type).HasConversion<string>();

        modelBuilder.Entity<RecipeDishType>().Property(dishType => dishType.Id).ValueGeneratedNever();

        modelBuilder.Entity<Recipe>().Property(recipe => recipe.CookTime).HasConversion<string>();
    }
}
