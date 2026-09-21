using Microsoft.Data.SqlClient;
using RealEstateProject.Models;

namespace RealEstateProject.Data
{
    public class ProjectsRepository
    {
        private readonly string _connectionString;

        public ProjectsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PartnersDB")
                ?? throw new InvalidOperationException("لم يتم العثور على Connection String");
        }

        // ============ المشاريع ============

        public async Task<List<Project>> GetProjectsAsync()
        {
            const string query = @"
                SELECT ProjectID, ProjectName, Location, CompanySharePercentage, 
                       InvestedAmount, PaymentMethod, StartDate, Status, Notes, CreatedAt
                FROM Projects
                ORDER BY ProjectID DESC";

            var result = new List<Project>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(MapProject(reader));
            }
            return result;
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            const string query = @"
                SELECT ProjectID, ProjectName, Location, CompanySharePercentage, 
                       InvestedAmount, PaymentMethod, StartDate, Status, Notes, CreatedAt
                FROM Projects WHERE ProjectID = @id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapProject(reader);
            return null;
        }

        public async Task AddProjectAsync(Project project)
        {
            const string query = @"
                INSERT INTO Projects (ProjectName, Location, CompanySharePercentage, 
                                      InvestedAmount, PaymentMethod, StartDate, Status, Notes)
                VALUES (@ProjectName, @Location, @CompanySharePercentage, 
                        @InvestedAmount, @PaymentMethod, @StartDate, @Status, @Notes)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProjectName", project.ProjectName);
            command.Parameters.AddWithValue("@Location", (object?)project.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompanySharePercentage", project.CompanySharePercentage);
            command.Parameters.AddWithValue("@InvestedAmount", project.InvestedAmount);
            command.Parameters.AddWithValue("@PaymentMethod", (object?)project.PaymentMethod ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", project.StartDate);
            command.Parameters.AddWithValue("@Status", (object?)project.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)project.Notes ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            const string query = @"
                UPDATE Projects
                SET ProjectName = @ProjectName, Location = @Location, 
                    CompanySharePercentage = @CompanySharePercentage, 
                    InvestedAmount = @InvestedAmount, PaymentMethod = @PaymentMethod, 
                    StartDate = @StartDate, Status = @Status, Notes = @Notes
                WHERE ProjectID = @ProjectID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProjectName", project.ProjectName);
            command.Parameters.AddWithValue("@Location", (object?)project.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompanySharePercentage", project.CompanySharePercentage);
            command.Parameters.AddWithValue("@InvestedAmount", project.InvestedAmount);
            command.Parameters.AddWithValue("@PaymentMethod", (object?)project.PaymentMethod ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", project.StartDate);
            command.Parameters.AddWithValue("@Status", (object?)project.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)project.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProjectID", project.ProjectID);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            const string query = "DELETE FROM Projects WHERE ProjectID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // ============ أقساط المشاريع ============

        public async Task<List<ProjectInstallment>> GetInstallmentsByProjectAsync(int projectId)
        {
            const string query = @"
                SELECT pi.InstallmentID, pi.ProjectID, p.ProjectName, pi.Amount, 
                       pi.DueDate, pi.PaidDate, pi.Status, pi.Notes
                FROM ProjectInstallments pi
                INNER JOIN Projects p ON pi.ProjectID = p.ProjectID
                WHERE pi.ProjectID = @ProjectID
                ORDER BY pi.DueDate";

            var result = new List<ProjectInstallment>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProjectID", projectId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new ProjectInstallment
                {
                    InstallmentID = reader.GetInt32(reader.GetOrdinal("InstallmentID")),
                    ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                    ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                    PaidDate = reader.IsDBNull(reader.GetOrdinal("PaidDate")) ? null : reader.GetDateTime(reader.GetOrdinal("PaidDate")),
                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                });
            }
            return result;
        }

        public async Task AddInstallmentAsync(ProjectInstallment installment)
        {
            const string query = @"
                INSERT INTO ProjectInstallments (ProjectID, Amount, DueDate, PaidDate, Status, Notes)
                VALUES (@ProjectID, @Amount, @DueDate, @PaidDate, @Status, @Notes)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProjectID", installment.ProjectID);
            command.Parameters.AddWithValue("@Amount", installment.Amount);
            command.Parameters.AddWithValue("@DueDate", installment.DueDate);
            command.Parameters.AddWithValue("@PaidDate", (object?)installment.PaidDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)installment.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)installment.Notes ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteInstallmentAsync(int id)
        {
            const string query = "DELETE FROM ProjectInstallments WHERE InstallmentID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // ============ Helper ============

        private static Project MapProject(SqlDataReader reader)
        {
            return new Project
            {
                ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                CompanySharePercentage = reader.GetDecimal(reader.GetOrdinal("CompanySharePercentage")),
                InvestedAmount = reader.GetDecimal(reader.GetOrdinal("InvestedAmount")),
                PaymentMethod = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? null : reader.GetString(reader.GetOrdinal("PaymentMethod")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            };
        }
    }
}