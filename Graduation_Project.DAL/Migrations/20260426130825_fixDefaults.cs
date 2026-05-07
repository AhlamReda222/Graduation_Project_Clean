using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fixDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "InAppEmails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Conversations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "InAppEmails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Conversations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "(NOW())",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "(NOW())");
        }
    }
}
