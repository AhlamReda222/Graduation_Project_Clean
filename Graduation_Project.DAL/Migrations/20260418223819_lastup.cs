using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class lastup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BrandDescription",
                table: "BrandOwnerRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrandLogoUrl",
                table: "BrandOwnerRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrandName",
                table: "BrandOwnerRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandDescription",
                table: "BrandOwnerRequests");

            migrationBuilder.DropColumn(
                name: "BrandLogoUrl",
                table: "BrandOwnerRequests");

            migrationBuilder.DropColumn(
                name: "BrandName",
                table: "BrandOwnerRequests");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
