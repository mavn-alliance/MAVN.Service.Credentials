using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AddPasswordResetEntitytTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordReset",
                schema: "credentials",
                table: "PasswordReset");

            migrationBuilder.RenameTable(
                name: "PasswordReset",
                schema: "credentials",
                newName: "password_reset",
                newSchema: "credentials");

            migrationBuilder.AddPrimaryKey(
                name: "PK_password_reset",
                schema: "credentials",
                table: "password_reset",
                column: "customer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_password_reset",
                schema: "credentials",
                table: "password_reset");

            migrationBuilder.RenameTable(
                name: "password_reset",
                schema: "credentials",
                newName: "PasswordReset",
                newSchema: "credentials");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordReset",
                schema: "credentials",
                table: "PasswordReset",
                column: "customer_id");
        }
    }
}
