using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AddPartnerIdIndexAndDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "partner_id",
                schema: "credentials",
                table: "partner_credentials",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(string));

            migrationBuilder.Sql("UPDATE [credentials].[partner_credentials] SET [partner_id] = newid();");

            migrationBuilder.CreateIndex(
                name: "IX_partner_credentials_partner_id",
                schema: "credentials",
                table: "partner_credentials",
                column: "partner_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_partner_credentials_partner_id",
                schema: "credentials",
                table: "partner_credentials");

            migrationBuilder.AlterColumn<string>(
                name: "partner_id",
                schema: "credentials",
                table: "partner_credentials",
                nullable: false,
                oldClrType: typeof(string),
                oldDefaultValueSql: "newid()");
        }
    }
}
