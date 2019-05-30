using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartElectionCertificationCentre.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivateKeys",
                columns: table => new
                {
                    CertificateThumbprint = table.Column<string>(nullable: false),
                    D = table.Column<string>(nullable: false),
                    DP = table.Column<string>(nullable: false),
                    DQ = table.Column<string>(nullable: false),
                    Exponent = table.Column<string>(nullable: false),
                    InverseQ = table.Column<string>(nullable: false),
                    Modulus = table.Column<string>(nullable: false),
                    P = table.Column<string>(nullable: false),
                    Q = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateKeys", x => x.CertificateThumbprint);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateKeys");
        }
    }
}
