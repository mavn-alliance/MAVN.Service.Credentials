using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AddRequiredFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "salt",
                schema: "credentials",
                table: "customer_credentials",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "hash",
                schema: "credentials",
                table: "customer_credentials",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "salt",
                schema: "credentials",
                table: "customer_credentials",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "hash",
                schema: "credentials",
                table: "customer_credentials",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
