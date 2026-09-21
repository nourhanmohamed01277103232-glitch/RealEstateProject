
namespace RealEstateProject.Models
{
    public class ProjectInstallment
    {
        public int InstallmentID { get; set; }
        public int ProjectID { get; set; }
        public string? ProjectName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
}