﻿namespace ExposureTracker.Models
{
    public class TranslationTables
    {
        [Key]
        public string plan_code { get; set; }
        [Required]

        public string ceding_company { get; set; }

        public string cedant_code { get; set; }

        public string benefit_cover { get; set; }

        public string insured_prod { get; set; }

        public string prod_description { get; set; }

        public string base_rider { get; set; }
    }
}
