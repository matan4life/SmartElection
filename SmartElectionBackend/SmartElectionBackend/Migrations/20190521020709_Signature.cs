using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartElectionBackend.Migrations
{
    public partial class Signature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionSign",
                table: "CommiteeAgreements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ElectionSign",
                table: "CommiteeAgreements",
                nullable: true);
        }
    }
}
