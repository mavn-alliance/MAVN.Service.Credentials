using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class AddPinCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "customer_id",
                schema: "credentials",
                table: "customer_credentials",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_customer_credentials_customer_id",
                schema: "credentials",
                table: "customer_credentials",
                column: "customer_id");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_pin_codes",
                schema: "credentials");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_customer_credentials_customer_id",
                schema: "credentials",
                table: "customer_credentials");

            migrationBuilder.AlterColumn<string>(
                name: "customer_id",
                schema: "credentials",
                table: "customer_credentials",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
