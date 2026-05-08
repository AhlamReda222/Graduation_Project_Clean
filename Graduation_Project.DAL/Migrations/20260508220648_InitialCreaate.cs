using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreaate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBehaviorEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    SessionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    BrandId = table.Column<int>(type: "integer", nullable: true),
                    SearchQuery = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SourcePage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBehaviorEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_BrandId",
                table: "UserBehaviorEvents",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_CategoryId",
                table: "UserBehaviorEvents",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_CreatedAt",
                table: "UserBehaviorEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_EventType",
                table: "UserBehaviorEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_ProductId",
                table: "UserBehaviorEvents",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_SessionId",
                table: "UserBehaviorEvents",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBehaviorEvents_UserId",
                table: "UserBehaviorEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBehaviorEvents");
        }
    }
}
