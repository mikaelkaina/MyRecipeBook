using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructure.DataAccess.Configurations.Recipes;

internal class RecipeDishTypeConfiguration : IEntityTypeConfiguration<RecipeDishType>
{
    public void Configure(EntityTypeBuilder<RecipeDishType> builder)
    {
        builder.ToTable("RecipeDishTypes");

        builder.HasKey(dt => dt.Id);

        builder.Property(dt => dt.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(dt => dt.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne<Recipe>()
            .WithMany(r => r.DishTypes)
            .HasForeignKey(dt => dt.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}