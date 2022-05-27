

using System.ComponentModel.DataAnnotations.Schema;

namespace ExposureTracker.Models
{
 
    public class Insured
    {
        [Key]
        public string Identifier { get; set; }
        [Required]
        public string PolicyNumber { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        
        public string FullNameDOB { get; set; }

        public string Gender { get; set; }

        public string ClientID { get; set; }

        public DateTime DateofBirth { get; set; }

        public string CedingCompany { get; set; }

        public string CedantCode { get; set; }

        public string TreatyCode { get; set; }

        public string Certificate { get; set; }

        public string Plan { get; set; }

        public string Currency { get; set; }

        public string Rider { get; set; }
        public DateTime PlanEffectiveDate { get; set; }

        public Decimal SumAssured { get; set; }

        public Decimal ReinsuredNetAmountAtRisk { get; set; }

        public Decimal ReinsuredNetAmountAtRiskPlan { get; set; }

        public Decimal ReinsuredNetAmountAtRiskRiders { get; set; }

        public string BordereauxYear { get; set; }

        public string SubstandardRatingPlan { get; set; }

        public string SubstandardRatingRiders { get; set; }

        public int RetrocededNarPlan { get; set; }

        public int RetrocededNarRider { get; set; }

        public string Status { get; set; }
    }


    public class fullnamedob
    {
        [Key]
        public string FullNameDOB {  get; set; }
    }
}
