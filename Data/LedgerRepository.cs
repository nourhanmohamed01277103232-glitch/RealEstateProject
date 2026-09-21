using Microsoft.Data.SqlClient;
using RealEstateProject.Models;

namespace RealEstateProject.Data
{
    // مسؤول عن قراءة بيانات دفتر الأستاذ من قاعدة AccountingDB
    public class LedgerRepository
    {
        private readonly string _connectionString;

        public LedgerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AccountingDB")
                ?? throw new InvalidOperationException("لم يتم العثور على Connection String باسم AccountingDB في appsettings.json");
        }

        // ==================== الدوال القديمة (قراءة فقط) ====================

        // دفتر الأستاذ التفصيلي - كل الحركات مع اسم ونوع الحساب
        public async Task<List<LedgerLine>> GetLedgerLinesAsync()
        {
            const string query = @"
                SELECT 
                    gl.EntryNumber,
                    gl.TransactionDate,
                    coa.AccountCode,
                    coa.AccountName,
                    coa.AccountType,
                    gl.Description,
                    gl.Reference,
                    gl.Debit,
                    gl.Credit
                FROM GeneralLedger gl
                INNER JOIN ChartOfAccounts coa ON gl.AccountID = coa.AccountID
                ORDER BY gl.TransactionDate, gl.EntryNumber, coa.AccountCode;";

            var result = new List<LedgerLine>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new LedgerLine
                {
                    EntryNumber = reader.GetString(reader.GetOrdinal("EntryNumber")),
                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                    AccountCode = reader.GetString(reader.GetOrdinal("AccountCode")),
                    AccountName = reader.GetString(reader.GetOrdinal("AccountName")),
                    AccountType = reader.GetString(reader.GetOrdinal("AccountType")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                    Reference = reader.IsDBNull(reader.GetOrdinal("Reference")) ? "" : reader.GetString(reader.GetOrdinal("Reference")),
                    Debit = reader.GetDecimal(reader.GetOrdinal("Debit")),
                    Credit = reader.GetDecimal(reader.GetOrdinal("Credit")),
                });
            }

            return result;
        }

        // ميزان المراجعة
        public async Task<List<TrialBalanceRow>> GetTrialBalanceAsync()
        {
            const string query = @"
                SELECT 
                    coa.AccountCode,
                    coa.AccountName,
                    SUM(gl.Debit) AS TotalDebit,
                    SUM(gl.Credit) AS TotalCredit
                FROM ChartOfAccounts coa
                LEFT JOIN GeneralLedger gl ON coa.AccountID = gl.AccountID
                GROUP BY coa.AccountCode, coa.AccountName
                HAVING SUM(gl.Debit) <> 0 OR SUM(gl.Credit) <> 0
                ORDER BY coa.AccountCode;";

            var result = new List<TrialBalanceRow>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new TrialBalanceRow
                {
                    AccountCode = reader.GetString(reader.GetOrdinal("AccountCode")),
                    AccountName = reader.GetString(reader.GetOrdinal("AccountName")),
                    TotalDebit = reader.IsDBNull(reader.GetOrdinal("TotalDebit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalDebit")),
                    TotalCredit = reader.IsDBNull(reader.GetOrdinal("TotalCredit")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalCredit")),
                });
            }

            return result;
        }

        // التأكد من أن إجمالي المدين = إجمالي الدائن
        public async Task<LedgerBalanceCheck> GetBalanceCheckAsync()
        {
            const string query = @"
                SELECT 
                    ISNULL(SUM(Debit), 0) AS TotalDebit,
                    ISNULL(SUM(Credit), 0) AS TotalCredit
                FROM GeneralLedger;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new LedgerBalanceCheck
                {
                    TotalDebit = reader.GetDecimal(reader.GetOrdinal("TotalDebit")),
                    TotalCredit = reader.GetDecimal(reader.GetOrdinal("TotalCredit")),
                };
            }

            return new LedgerBalanceCheck();
        }

        // ==================== الدوال الجديدة (إضافة وتعديل وحذف) ====================

        // 1. جيب كل الحسابات للـ Dropdown
        public async Task<List<AccountDropdownItem>> GetAccountsAsync()
        {
            const string query = @"
                SELECT AccountID, AccountCode, AccountName, AccountType
                FROM ChartOfAccounts
                ORDER BY AccountCode;";

            var result = new List<AccountDropdownItem>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new AccountDropdownItem
                {
                    AccountID = reader.GetInt32(reader.GetOrdinal("AccountID")),
                    AccountCode = reader.GetString(reader.GetOrdinal("AccountCode")),
                    AccountName = reader.GetString(reader.GetOrdinal("AccountName")),
                    AccountType = reader.GetString(reader.GetOrdinal("AccountType")),
                });
            }
            return result;
        }

        // 2. إضافة قيد جديد (طرفين أو أكثر)
        public async Task AddJournalEntryAsync(JournalEntry entry)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // التحقق من التوازن
                var totalDebit = entry.Lines.Sum(l => l.Debit);
                var totalCredit = entry.Lines.Sum(l => l.Credit);
                if (totalDebit != totalCredit)
                    throw new InvalidOperationException("القيد غير متوازن: إجمالي المدين لا يساوي إجمالي الدائن");

                const string insertQuery = @"
                    INSERT INTO GeneralLedger 
                        (EntryNumber, TransactionDate, AccountID, Description, Reference, Debit, Credit)
                    VALUES 
                        (@EntryNumber, @TransactionDate, @AccountID, @Description, @Reference, @Debit, @Credit)";

                foreach (var line in entry.Lines)
                {
                    using var command = new SqlCommand(insertQuery, connection, transaction);
                    command.Parameters.AddWithValue("@EntryNumber", entry.EntryNumber);
                    command.Parameters.AddWithValue("@TransactionDate", entry.TransactionDate);
                    command.Parameters.AddWithValue("@AccountID", line.AccountID);
                    command.Parameters.AddWithValue("@Description", (object?)line.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Reference", (object?)line.Reference ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Debit", line.Debit);
                    command.Parameters.AddWithValue("@Credit", line.Credit);
                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // 3. حذف قيد بالكامل (كل السطور اللي ليه نفس EntryNumber)
        public async Task DeleteJournalEntryAsync(string entryNumber)
        {
            const string query = "DELETE FROM GeneralLedger WHERE EntryNumber = @EntryNumber";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EntryNumber", entryNumber);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // 4. جيب قيد واحد بالكامل عشان تعدليه
        public async Task<JournalEntry?> GetJournalEntryByNumberAsync(string entryNumber)
        {
            const string query = @"
                SELECT gl.EntryNumber, gl.TransactionDate, gl.AccountID, 
                       coa.AccountCode, coa.AccountName,
                       gl.Description, gl.Reference, gl.Debit, gl.Credit
                FROM GeneralLedger gl
                INNER JOIN ChartOfAccounts coa ON gl.AccountID = coa.AccountID
                WHERE gl.EntryNumber = @EntryNumber
                ORDER BY gl.Debit DESC;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EntryNumber", entryNumber);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            JournalEntry? entry = null;
            while (await reader.ReadAsync())
            {
                if (entry == null)
                {
                    entry = new JournalEntry
                    {
                        EntryNumber = reader.GetString(reader.GetOrdinal("EntryNumber")),
                        TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                        Reference = reader.IsDBNull(reader.GetOrdinal("Reference")) ? "" : reader.GetString(reader.GetOrdinal("Reference")),
                    };
                }

                entry.Lines.Add(new JournalLine
                {
                    EntryNumber = reader.GetString(reader.GetOrdinal("EntryNumber")),
                    TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                    AccountID = reader.GetInt32(reader.GetOrdinal("AccountID")),
                    AccountCode = reader.GetString(reader.GetOrdinal("AccountCode")),
                    AccountName = reader.GetString(reader.GetOrdinal("AccountName")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                    Reference = reader.IsDBNull(reader.GetOrdinal("Reference")) ? "" : reader.GetString(reader.GetOrdinal("Reference")),
                    Debit = reader.GetDecimal(reader.GetOrdinal("Debit")),
                    Credit = reader.GetDecimal(reader.GetOrdinal("Credit")),
                });
            }

            return entry;
        }

        // 5. تعديل قيد (بحذف القديم وإضافة الجديد في Transaction واحدة)
        public async Task UpdateJournalEntryAsync(string oldEntryNumber, JournalEntry newEntry)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // احذف القيد القديم
                using (var deleteCmd = new SqlCommand("DELETE FROM GeneralLedger WHERE EntryNumber = @EntryNumber", connection, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("@EntryNumber", oldEntryNumber);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                // أضف القيد الجديد
                var totalDebit = newEntry.Lines.Sum(l => l.Debit);
                var totalCredit = newEntry.Lines.Sum(l => l.Credit);
                if (totalDebit != totalCredit)
                    throw new InvalidOperationException("القيد غير متوازن");

                const string insertQuery = @"
                    INSERT INTO GeneralLedger 
                        (EntryNumber, TransactionDate, AccountID, Description, Reference, Debit, Credit)
                    VALUES 
                        (@EntryNumber, @TransactionDate, @AccountID, @Description, @Reference, @Debit, @Credit)";

                foreach (var line in newEntry.Lines)
                {
                    using var command = new SqlCommand(insertQuery, connection, transaction);
                    command.Parameters.AddWithValue("@EntryNumber", newEntry.EntryNumber);
                    command.Parameters.AddWithValue("@TransactionDate", newEntry.TransactionDate);
                    command.Parameters.AddWithValue("@AccountID", line.AccountID);
                    command.Parameters.AddWithValue("@Description", (object?)line.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Reference", (object?)line.Reference ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Debit", line.Debit);
                    command.Parameters.AddWithValue("@Credit", line.Credit);
                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}