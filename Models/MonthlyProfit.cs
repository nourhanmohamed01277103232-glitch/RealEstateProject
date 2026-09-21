namespace RealEstateProject.Models
{
    public class MonthlyProfit
    {
        public int MonthlyProfitID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProfitDistribution> Distributions { get; set; } = new();
    }
}