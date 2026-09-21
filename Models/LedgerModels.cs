namespace RealEstateProject.Models
{
    // صف واحد من "دفتر الأستاذ التفصيلي" (كل الحركات)
    public class LedgerLine
    {
        public string EntryNumber { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string AccountType { get; set; } = "";
        public string Description { get; set; } = "";
        public string Reference { get; set; } = "";
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal NetMovement => Debit - Credit;
    }

    // صف واحد من "ميزان المراجعة"
    public class TrialBalanceRow
    {
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }

    // ملخص التحقق من توازن الدفتر (مدين = دائن)
    public class LedgerBalanceCheck
    {
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Difference => TotalDebit - TotalCredit;
        public bool IsBalanced => Difference == 0;
    }

    // الشكل الكامل اللي بيتبعت للصفحة
    public class LedgerViewModel
    {
        public List<LedgerLine> Lines { get; set; } = new();
        public List<TrialBalanceRow> TrialBalance { get; set; } = new();
        public LedgerBalanceCheck BalanceCheck { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}



namespace RealEstateProject.Models
{
    // ... (الكود القديم بتاعك زي ما هو)

    // ============ Models جديدة للإضافة والتعديل ============

    // سطر واحد في القيد (الطرف المدين أو الدائن)
    public class JournalLine
    {
        public int LineID { get; set; }
        public string EntryNumber { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public int AccountID { get; set; }
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Reference { get; set; } = "";
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    // قيد كامل (طرفين: مدين + دائن)
    public class JournalEntry
    {
        public string EntryNumber { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = "";
        public string Reference { get; set; } = "";
        public List<JournalLine> Lines { get; set; } = new();
    }

    // Model لعرض الحسابات في الـ Dropdown
    public class AccountDropdownItem
    {
        public int AccountID { get; set; }
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string AccountType { get; set; } = "";
    }
}