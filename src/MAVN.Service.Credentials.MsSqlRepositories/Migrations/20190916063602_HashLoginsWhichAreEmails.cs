using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Credentials.MsSqlRepositories.Migrations
{
    public partial class HashLoginsWhichAreEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE [credentials].[customer_credentials]
                                    SET [login] = CONVERT(nvarchar(450), hashbytes('SHA2_256', CONVERT(varchar(450),[login])), 2)");

            migrationBuilder.Sql(@"UPDATE [credentials].[admin_credentials]
                                    SET [login] = CONVERT(nvarchar(450), hashbytes('SHA2_256', CONVERT(varchar(450),[login])), 2)");

            migrationBuilder.Sql(@"UPDATE [credentials].[partner_credentials]
                                    SET [client_id] = CONVERT(nvarchar(450), hashbytes('SHA2_256', CONVERT(varchar(450),[client_id])), 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
