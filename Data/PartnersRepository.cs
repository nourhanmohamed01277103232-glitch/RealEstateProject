using Microsoft.Data.SqlClient;
using RealEstateProject.Models;

namespace RealEstateProject.Data
{
    public class PartnersRepository
    {
        private readonly string _connectionString;

        public PartnersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PartnersDB")
                ?? throw new InvalidOperationException("لم يتم العثور على Connection String باسم PartnersDB");
        }

        public async Task<List<Partner>> GetPartnersAsync()
        {
            const string query = @"
                SELECT 
                    p.PartnerID, p.Name, p.OwnershipPercentage, p.PhoneNumber, p.JoinDate,
                    ISNULL((SELECT SUM(Amount) FROM CapitalMovements WHERE PartnerID = p.PartnerID AND MovementType = N'إيداع'), 0) AS TotalDeposits,
                    ISNULL((SELECT SUM(Amount) FROM CapitalMovements WHERE PartnerID = p.PartnerID AND MovementType = N'سحب'), 0) AS TotalWithdrawals
                FROM Partners p
                ORDER BY p.PartnerID;";

            var result = new List<Partner>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new Partner
                {
                    PartnerID = reader.GetInt32(reader.GetOrdinal("PartnerID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    OwnershipPercentage = reader.GetDecimal(reader.GetOrdinal("OwnershipPercentage")),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    JoinDate = reader.GetDateTime(reader.GetOrdinal("JoinDate")),
                    TotalDeposits = reader.GetDecimal(reader.GetOrdinal("TotalDeposits")),
                    TotalWithdrawals = reader.GetDecimal(reader.GetOrdinal("TotalWithdrawals")),
                });
            }
            return result;
        }

        public async Task<Partner?> GetPartnerByIdAsync(int id)
        {
            const string query = "SELECT PartnerID, Name, OwnershipPercentage, PhoneNumber, JoinDate FROM Partners WHERE PartnerID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Partner
                {
                    PartnerID = reader.GetInt32(reader.GetOrdinal("PartnerID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    OwnershipPercentage = reader.GetDecimal(reader.GetOrdinal("OwnershipPercentage")),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    JoinDate = reader.GetDateTime(reader.GetOrdinal("JoinDate")),
                };
            }
            return null;
        }

        public async Task AddPartnerAsync(Partner partner)
        {
            const string query = @"
                INSERT INTO Partners (Name, OwnershipPercentage, PhoneNumber, JoinDate)
                VALUES (@Name, @OwnershipPercentage, @PhoneNumber, @JoinDate)";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", partner.Name);
            command.Parameters.AddWithValue("@OwnershipPercentage", partner.OwnershipPercentage);
            command.Parameters.AddWithValue("@PhoneNumber", (object?)partner.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@JoinDate", partner.JoinDate);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdatePartnerAsync(Partner partner)
        {
            const string query = @"
                UPDATE Partners
                SET Name = @Name, OwnershipPercentage = @OwnershipPercentage, PhoneNumber = @PhoneNumber, JoinDate = @JoinDate
                WHERE PartnerID = @PartnerID";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", partner.Name);
            command.Parameters.AddWithValue("@OwnershipPercentage", partner.OwnershipPercentage);
            command.Parameters.AddWithValue("@PhoneNumber", (object?)partner.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@JoinDate", partner.JoinDate);
            command.Parameters.AddWithValue("@PartnerID", partner.PartnerID);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeletePartnerAsync(int id)
        {
            const string query = "DELETE FROM Partners WHERE PartnerID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<CapitalMovement>> GetMovementsAsync()
        {
            const string query = @"
                SELECT cm.MovementID, cm.PartnerID, p.Name AS PartnerName, cm.MovementType, cm.Amount, cm.MovementDate, cm.Notes
                FROM CapitalMovements cm
                INNER JOIN Partners p ON cm.PartnerID = p.PartnerID
                ORDER BY cm.MovementDate DESC, cm.MovementID DESC";

            var result = new List<CapitalMovement>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CapitalMovement
                {
                    MovementID = reader.GetInt32(reader.GetOrdinal("MovementID")),
                    PartnerID = reader.GetInt32(reader.GetOrdinal("PartnerID")),
                    PartnerName = reader.GetString(reader.GetOrdinal("PartnerName")),
                    MovementType = reader.GetString(reader.GetOrdinal("MovementType")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    MovementDate = reader.GetDateTime(reader.GetOrdinal("MovementDate")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                });
            }
            return result;
        }

        public async Task AddMovementAsync(CapitalMovement movement)
        {
            const string query = @"
                INSERT INTO CapitalMovements (PartnerID, MovementType, Amount, MovementDate, Notes)
                VALUES (@PartnerID, @MovementType, @Amount, @MovementDate, @Notes)";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PartnerID", movement.PartnerID);
            command.Parameters.AddWithValue("@MovementType", movement.MovementType);
            command.Parameters.AddWithValue("@Amount", movement.Amount);
            command.Parameters.AddWithValue("@MovementDate", movement.MovementDate);
            command.Parameters.AddWithValue("@Notes", (object?)movement.Notes ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteMovementAsync(int id)
        {
            const string query = "DELETE FROM CapitalMovements WHERE MovementID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<CompanyExpense>> GetExpensesAsync()
        {
            const string query = "SELECT ExpenseID, Description, Amount, ExpenseDate FROM CompanyExpenses ORDER BY ExpenseDate DESC, ExpenseID DESC";
            var result = new List<CompanyExpense>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new CompanyExpense
                {
                    ExpenseID = reader.GetInt32(reader.GetOrdinal("ExpenseID")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    ExpenseDate = reader.GetDateTime(reader.GetOrdinal("ExpenseDate")),
                });
            }
            return result;
        }

        public async Task<List<ExpenseShare>> GetExpenseSharesAsync(int expenseId)
        {
            const string query = @"
                SELECT es.ShareID, es.ExpenseID, ce.Description AS ExpenseDescription, ce.ExpenseDate, es.PartnerID, p.Name AS PartnerName, es.ShareAmount
                FROM ExpenseShares es
                INNER JOIN CompanyExpenses ce ON es.ExpenseID = ce.ExpenseID
                INNER JOIN Partners p ON es.PartnerID = p.PartnerID
                WHERE es.ExpenseID = @ExpenseID
                ORDER BY p.PartnerID";
            var result = new List<ExpenseShare>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ExpenseID", expenseId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new ExpenseShare
                {
                    ShareID = reader.GetInt32(reader.GetOrdinal("ShareID")),
                    ExpenseID = reader.GetInt32(reader.GetOrdinal("ExpenseID")),
                    ExpenseDescription = reader.GetString(reader.GetOrdinal("ExpenseDescription")),
                    ExpenseDate = reader.GetDateTime(reader.GetOrdinal("ExpenseDate")),
                    PartnerID = reader.GetInt32(reader.GetOrdinal("PartnerID")),
                    PartnerName = reader.GetString(reader.GetOrdinal("PartnerName")),
                    ShareAmount = reader.GetDecimal(reader.GetOrdinal("ShareAmount")),
                });
            }
            return result;
        }

        public async Task AddExpenseAsync(CompanyExpense expense)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // 1. جيب الشركاء باستخدام نفس الـ Connection
            var partners = new List<Partner>();
            const string getPartnersQuery = "SELECT PartnerID, Name, OwnershipPercentage FROM Partners";
            using (var partnersCommand = new SqlCommand(getPartnersQuery, connection))
            using (var reader = await partnersCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    partners.Add(new Partner
                    {
                        PartnerID = reader.GetInt32(reader.GetOrdinal("PartnerID")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        OwnershipPercentage = reader.GetDecimal(reader.GetOrdinal("OwnershipPercentage")),
                    });
                }
            }

            // 2. أدخل المصروف
            const string insertExpense = @"
        INSERT INTO CompanyExpenses (Description, Amount, ExpenseDate)
        OUTPUT INSERTED.ExpenseID
        VALUES (@Description, @Amount, @ExpenseDate)";
            using var expenseCommand = new SqlCommand(insertExpense, connection);
            expenseCommand.Parameters.AddWithValue("@Description", expense.Description);
            expenseCommand.Parameters.AddWithValue("@Amount", expense.Amount);
            expenseCommand.Parameters.AddWithValue("@ExpenseDate", expense.ExpenseDate);
            var newExpenseId = (int)(await expenseCommand.ExecuteScalarAsync())!;

            // 3. وزع المصروف على الشركاء
            foreach (var partner in partners)
            {
                var share = expense.Amount * (partner.OwnershipPercentage / 100m);
                const string insertShare = @"
            INSERT INTO ExpenseShares (ExpenseID, PartnerID, ShareAmount)
            VALUES (@ExpenseID, @PartnerID, @ShareAmount)";
                using var shareCommand = new SqlCommand(insertShare, connection);
                shareCommand.Parameters.AddWithValue("@ExpenseID", newExpenseId);
                shareCommand.Parameters.AddWithValue("@PartnerID", partner.PartnerID);
                shareCommand.Parameters.AddWithValue("@ShareAmount", share);
                await shareCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteExpenseAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var deleteShares = new SqlCommand("DELETE FROM ExpenseShares WHERE ExpenseID = @id", connection);
            deleteShares.Parameters.AddWithValue("@id", id);
            await deleteShares.ExecuteNonQueryAsync();

            using var deleteExpense = new SqlCommand("DELETE FROM CompanyExpenses WHERE ExpenseID = @id", connection);
            deleteExpense.Parameters.AddWithValue("@id", id);
            await deleteExpense.ExecuteNonQueryAsync();
        }
    }
}