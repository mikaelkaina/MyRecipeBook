using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructure.DataAcess.Configurations;

internal class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");

        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ri => ri.Item)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasOne<Recipe>()
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}