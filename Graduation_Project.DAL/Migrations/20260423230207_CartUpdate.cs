using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CartUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomizationZone",
                table: "CartItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignImageUrl",
                table: "CartItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DesignText",
                table: "CartItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "CartItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TechniqueId",
                table: "CartItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_TechniqueId",
                table: "CartItems",
                column: "TechniqueId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_PrintingTechniques_TechniqueId",
                table: "CartItems",
                column: "TechniqueId",
                principalTable: "PrintingTechniques",
                principalColumn: "TechniqueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_PrintingTechniques_TechniqueId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_TechniqueId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CustomizationZone",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DesignImageUrl",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DesignText",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "TechniqueId",
                table: "CartItems");
        }
    }
}
