using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExposureTracker.Migrations
{
    public partial class UwDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dbLifeData",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identifier = table.Column<string>(type: "text", nullable: false),
                    policyno = table.Column<string>(type: "text", nullable: false),
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
                    baserider = table.Column<string>(type: "text", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    planeffectivedate = table.Column<string>(type: "text", nullable: false),
                    sumassured = table.Column<decimal>(type: "numeric", nullable: false),
                    reinsurednetamountatrisk = table.Column<decimal>(type: "numeric", nullable: false),
                    mortalityrating = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    dateuploaded = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbLifeData", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dbTranslationTable",
                columns: table => new
                {
                    plan_code = table.Column<string>(type: "text", nullable: false),
                    ceding_company = table.Column<string>(type: "text", nullable: false),
                    cedant_code = table.Column<string>(type: "text", nullable: false),
                    benefit_cover = table.Column<string>(type: "text", nullable: false),
                    insured_prod = table.Column<string>(type: "text", nullable: false),
                    prod_description = table.Column<string>(type: "text", nullable: false),
                    base_rider = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbTranslationTable", x => x.plan_code);
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
