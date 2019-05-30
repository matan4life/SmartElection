using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartElectionBackend.Migrations
{
    public partial class Certificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElectionId",
                table: "UserCertificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserCertificates_ElectionId",
                table: "UserCertificates",
                column: "ElectionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificates_Elections_ElectionId",
                table: "UserCertificates",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificates_Elections_ElectionId",
                table: "UserCertificates");

            migrationBuilder.DropIndex(
                name: "IX_UserCertificates_ElectionId",
                table: "UserCertificates");

            migrationBuilder.DropColumn(
                name: "ElectionId",
                table: "UserCertificates");
        }
    }
}
