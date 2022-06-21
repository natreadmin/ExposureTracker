namespace ExposureTracker.Models
{
    public class TranslationTables
    {
        [Key]
        public string plancode { get; set; }
        [Required]

        public string cedingcompany { get; set; }

        public string benefitcov { get; set; }

        public string insuredprod { get; set; }
    }
}
