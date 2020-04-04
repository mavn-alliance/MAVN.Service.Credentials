using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AddPartnerCredentialsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "partner_credentials",
                schema: "credentials",
                columns: table => new
                {
                    client_id = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    hash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partner_credentials", x => x.client_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "partner_credentials",
                schema: "credentials");
        }
    }
}
