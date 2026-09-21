namespace RealEstateProject.Models
{
    public class ProjectProfit
    {
        public int ProfitID { get; set; }
        public int ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public decimal Amount { get; set; }
        public DateTime ProfitDate { get; set; }
        public string? Notes { get; set; }
    }
}