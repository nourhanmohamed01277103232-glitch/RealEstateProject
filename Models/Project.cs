namespace RealEstateProject.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public decimal CompanySharePercentage { get; set; }
        public decimal InvestedAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime StartDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}