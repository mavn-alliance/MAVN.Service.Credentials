using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "credentials");

            migrationBuilder.CreateTable(
                name: "admin_credentials",
                schema: "credentials",
                columns: table => new
                {
                    login = table.Column<string>(nullable: false),
                    admin_id = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    hash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_credentials", x => x.login);
                });

            migrationBuilder.CreateTable(
                name: "customer_credentials",
                schema: "credentials",
                columns: table => new
                {
                    login = table.Column<string>(nullable: false),
                    customer_id = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    hash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_credentials", x => x.login);
                    table.UniqueConstraint("AK_customer_credentials_customer_id", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "partner_credentials",
                schema: "credentials",
                columns: table => new
                {
                    partner_id = table.Column<string>(nullable: false),
                    client_id = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    hash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partner_credentials", x => x.partner_id);
                });

            migrationBuilder.CreateTable(
                name: "password_reset",
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
                    table.PrimaryKey("PK_password_reset", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "customer_pin_codes",
                schema: "credentials",
                columns: table => new
                {
                    customer_id = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    hash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_pin_codes", x => x.customer_id);
                    table.ForeignKey(
                        name: "FK_customer_pin_codes_customer_credentials_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "credentials",
                        principalTable: "customer_credentials",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_partner_credentials_partner_id",
                schema: "credentials",
                table: "partner_credentials",
                column: "partner_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_credentials",
                schema: "credentials");

            migrationBuilder.DropTable(
                name: "customer_pin_codes",
                schema: "credentials");

            migrationBuilder.DropTable(
                name: "partner_credentials",
                schema: "credentials");

            migrationBuilder.DropTable(
                name: "password_reset",
                schema: "credentials");

            migrationBuilder.DropTable(
                name: "customer_credentials",
                schema: "credentials");
        }
    }
}
