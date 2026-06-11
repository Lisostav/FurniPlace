using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurniPlace.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToFurniture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "FurnitureItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "FurnitureItems");
        }
    }
}
