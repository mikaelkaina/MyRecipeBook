using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructure.DataAcess.Configurations;

internal class RecipeInstructionConfiguration : IEntityTypeConfiguration<RecipeInstruction>
{
    public void Configure(EntityTypeBuilder<RecipeInstruction> builder)
    {
        builder.ToTable("RecipeInstructions");

        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ri => ri.Order)
            .IsRequired();

        builder.Property(ri => ri.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne<Recipe>()
            .WithMany(r => r.Instructions)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}