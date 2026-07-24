using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructure.DataAcess.Configurations;

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
            .HasConversion<int>();

        builder.HasOne<Recipe>()
            .WithMany(r => r.DishTypes)
            .HasForeignKey(dt => dt.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}