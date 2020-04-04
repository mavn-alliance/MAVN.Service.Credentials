using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AddPartnerIdToPartnerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "partner_id",
                schema: "credentials",
                table: "partner_credentials",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "partner_id",
                schema: "credentials",
                table: "partner_credentials");
        }
    }
}
