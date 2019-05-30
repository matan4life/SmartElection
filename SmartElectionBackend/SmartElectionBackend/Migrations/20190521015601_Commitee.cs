using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartElectionBackend.Migrations
{
    public partial class Commitee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificates_Elections_ElectionId",
                table: "UserCertificates");

            migrationBuilder.DropIndex(
                name: "IX_UserCertificates_ElectionId",
                table: "UserCertificates");

            migrationBuilder.AlterColumn<int>(
                name: "ElectionId",
                table: "UserCertificates",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ElectoralCommiteeId",
                table: "UserCertificates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCertificates_ElectionId",
                table: "UserCertificates",
                column: "ElectionId",
                unique: true,
                filter: "[ElectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserCertificates_ElectoralCommiteeId",
                table: "UserCertificates",
                column: "ElectoralCommiteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificates_Elections_ElectionId",
                table: "UserCertificates",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCertificates_ElectoralCommitees_ElectoralCommiteeId",
                table: "UserCertificates",
                column: "ElectoralCommiteeId",
                principalTable: "ElectoralCommitees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificates_Elections_ElectionId",
                table: "UserCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCertificates_ElectoralCommitees_ElectoralCommiteeId",
                table: "UserCertificates");

            migrationBuilder.DropIndex(
                name: "IX_UserCertificates_ElectionId",
                table: "UserCertificates");

            migrationBuilder.DropIndex(
                name: "IX_UserCertificates_ElectoralCommiteeId",
                table: "UserCertificates");

            migrationBuilder.DropColumn(
                name: "ElectoralCommiteeId",
                table: "UserCertificates");

            migrationBuilder.AlterColumn<int>(
                name: "ElectionId",
                table: "UserCertificates",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
    }
}
