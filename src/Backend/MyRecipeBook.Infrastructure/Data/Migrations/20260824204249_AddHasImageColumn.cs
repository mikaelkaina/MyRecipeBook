using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyRecipeBook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHasImageColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasImage",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasImage",
                table: "Recipes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasImage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HasImage",
                table: "Recipes");
        }
    }
}
