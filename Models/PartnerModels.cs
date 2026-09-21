namespace RealEstateProject.Models
{
    public class Partner
    {
        public int PartnerID { get; set; }
        public string Name { get; set; } = "";
        public decimal OwnershipPercentage { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime JoinDate { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal NetBalance => TotalDeposits - TotalWithdrawals;
    }


    public class CapitalMovement
    {
        public int MovementID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerName { get; set; } = "";
        public string MovementType { get; set; } = "إيداع";
        public decimal Amount { get; set; }
        public DateTime MovementDate { get; set; }
        public string? Notes { get; set; }
    }


    public class CompanyExpense
    {
        public int ExpenseID { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }

    public class ExpenseShare
    {
        public int ShareID { get; set; }
        public int ExpenseID { get; set; }
        public string ExpenseDescription { get; set; } = "";
        public DateTime ExpenseDate { get; set; }
        public int PartnerID { get; set; }
        public string PartnerName { get; set; } = "";
        public decimal ShareAmount { get; set; }
    }
}