using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExposureTracker.Migrations
{
    public partial class UWTranslationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dbLifeData",
                columns: table => new
                {
                    policyno = table.Column<string>(type: "text", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    middlename = table.Column<string>(type: "text", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    fullName = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    clientid = table.Column<string>(type: "text", nullable: false),
                    dateofbirth = table.Column<string>(type: "text", nullable: false),
                    cedingcompany = table.Column<string>(type: "text", nullable: false),
                    cedantcode = table.Column<string>(type: "text", nullable: false),
                    typeofbusiness = table.Column<string>(type: "text", nullable: false),
                    bordereauxfilename = table.Column<string>(type: "text", nullable: false),
                    bordereauxyear = table.Column<int>(type: "integer", nullable: false),
                    certificate = table.Column<string>(type: "text", nullable: false),
                    plan = table.Column<string>(type: "text", nullable: false),
                    benefittype = table.Column<string>(type: "text", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    planeffectivedate = table.Column<string>(type: "text", nullable: false),
                    sumassured = table.Column<decimal>(type: "numeric", nullable: false),
                    reinsurednetamountatrisk = table.Column<decimal>(type: "numeric", nullable: false),
                    mortalityrating = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbLifeData", x => x.policyno);
                });

            migrationBuilder.CreateTable(
                name: "dbTranslationTable",
                columns: table => new
                {
                    plancode = table.Column<string>(type: "text", nullable: false),
                    cedingcompany = table.Column<string>(type: "text", nullable: false),
                    benefitcov = table.Column<string>(type: "text", nullable: false),
                    insuredprod = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbTranslationTable", x => x.plancode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbLifeData");

            migrationBuilder.DropTable(
                name: "dbTranslationTable");
        }
    }
}
