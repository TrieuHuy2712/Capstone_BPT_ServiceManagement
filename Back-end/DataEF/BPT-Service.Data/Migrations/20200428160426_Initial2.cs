using Microsoft.EntityFrameworkCore.Migrations;

namespace BPT_Service.Data.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codeConfirm",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeConfirm",
                table: "ProviderNews",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "codeConfirm",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CodeConfirm",
                table: "ProviderNews");
        }
    }
}
