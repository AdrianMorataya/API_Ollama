using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OllamaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailConfirmationToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationCode",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2025, 10, 15, 21, 2, 45, 499, DateTimeKind.Utc).AddTicks(7823), "admin@ollama.com", true, "$2a$11$4dbKBQ2hCZxaCO9JQmwjO.2CL.Rkhlpbr.LkjYG5S9bKFRBBEcKFS", "Admin", new DateTime(2025, 10, 15, 21, 2, 45, 499, DateTimeKind.Utc).AddTicks(7967), "admin" });
        }
    }
}
