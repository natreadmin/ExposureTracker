using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExposureTracker.Migrations
{
    public partial class AddExposureData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dbInsured",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    PolicyNumber = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    ClientID = table.Column<string>(type: "text", nullable: false),
                    DateofBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CedingCompany = table.Column<string>(type: "text", nullable: false),
                    CedantCode = table.Column<string>(type: "text", nullable: false),
                    TreatyCode = table.Column<string>(type: "text", nullable: false),
                    Certificate = table.Column<string>(type: "text", nullable: false),
                    Plan = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Rider = table.Column<string>(type: "text", nullable: false),
                    PlanEffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SumAssured = table.Column<decimal>(type: "numeric", nullable: false),
                    ReinsuredNetAmountAtRisk = table.Column<decimal>(type: "numeric", nullable: false),
                    ReinsuredNetAmountAtRiskPlan = table.Column<decimal>(type: "numeric", nullable: false),
                    ReinsuredNetAmountAtRiskRiders = table.Column<decimal>(type: "numeric", nullable: false),
                    BordereauxYear = table.Column<string>(type: "text", nullable: false),
                    SubstandardRatingPlan = table.Column<string>(type: "text", nullable: false),
                    SubstandardRatingRiders = table.Column<string>(type: "text", nullable: false),
                    RetrocededNarPlan = table.Column<string>(type: "text", nullable: false),
                    RetrocededNarRider = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbInsured", x => new { x.Id, x.PolicyNumber });
                });

            migrationBuilder.CreateTable(
                name: "dbPolicy",
                columns: table => new
                {
                    PolicyNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbPolicy", x => x.PolicyNumber);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dbInsured");

            migrationBuilder.DropTable(
                name: "dbPolicy");
        }
    }
}
