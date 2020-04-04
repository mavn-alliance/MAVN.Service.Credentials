using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AllowPartnerClientIdChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_partner_credentials",
                schema: "credentials",
                table: "partner_credentials");

            migrationBuilder.AlterColumn<string>(
                name: "client_id",
                schema: "credentials",
                table: "partner_credentials",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_partner_credentials",
                schema: "credentials",
                table: "partner_credentials",
                column: "partner_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_partner_credentials",
                schema: "credentials",
                table: "partner_credentials");

            migrationBuilder.AlterColumn<string>(
                name: "client_id",
                schema: "credentials",
                table: "partner_credentials",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_partner_credentials",
                schema: "credentials",
                table: "partner_credentials",
                column: "client_id");
        }
    }
}
