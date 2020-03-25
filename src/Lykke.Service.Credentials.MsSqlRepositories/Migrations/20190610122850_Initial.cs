using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "credentials");

            migrationBuilder.CreateTable(
                name: "customer_credentials",
                schema: "credentials",
                columns: table => new
                {
                    login = table.Column<string>(nullable: false),
                    customer_id = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: true),
                    hash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_credentials", x => x.login);
                });

            migrationBuilder.CreateTable(
                name: "PasswordReset",
                schema: "credentials",
                columns: table => new
                {
                    customer_id = table.Column<string>(nullable: false),
                    identifier = table.Column<string>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    expires_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordReset", x => x.customer_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_credentials",
                schema: "credentials");

            migrationBuilder.DropTable(
                name: "PasswordReset",
                schema: "credentials");
        }
    }
}
