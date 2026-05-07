using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowsPrinting",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowsText",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowsPrinting",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AllowsText",
                table: "Products");
        }
    }
}
