using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AdminCredentials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_credentials",
                schema: "credentials");
        }
    }
}
