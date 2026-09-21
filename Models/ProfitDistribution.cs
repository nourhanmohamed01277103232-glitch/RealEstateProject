namespace RealEstateProject.Models
{
    public class ProfitDistribution
    {
        public int DistributionID { get; set; }
        public int MonthlyProfitID { get; set; }
        public int PartnerID { get; set; }
        public string? PartnerName { get; set; }
        public decimal ShareAmount { get; set; }
    }
}