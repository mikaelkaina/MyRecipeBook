using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructure.DataAccess.Configurations;

internal class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(r => r.CookTime)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.HasImage)
            .IsRequired()
            .HasDefaultValue(false);
    }
}