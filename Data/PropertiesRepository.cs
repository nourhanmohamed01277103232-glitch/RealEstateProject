using Microsoft.Data.SqlClient;
using RealEstateProject.Models;

namespace RealEstateProject.Data
{
    public class PropertiesRepository
    {
        private readonly string _connectionString;

        public PropertiesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PartnersDB")
                ?? throw new InvalidOperationException("لم يتم العثور على Connection String");
        }

        public async Task<List<Property>> SearchAsync(
            decimal? priceFrom, decimal? priceTo,
            string? propertyType, string? paymentMethod,
            DateTime? deliveryDate, string? activity)
        {
            var query = "SELECT * FROM Properties WHERE IsAvailable = 1";
            var parameters = new List<SqlParameter>();

            if (priceFrom.HasValue)
            {
                query += " AND Price >= @PriceFrom";
                parameters.Add(new SqlParameter("@PriceFrom", priceFrom.Value));
            }

            if (priceTo.HasValue)
            {
                query += " AND Price <= @PriceTo";
                parameters.Add(new SqlParameter("@PriceTo", priceTo.Value));
            }

            if (!string.IsNullOrEmpty(propertyType) && propertyType != "الكل")
            {
                query += " AND PropertyType = @PropertyType";
                parameters.Add(new SqlParameter("@PropertyType", propertyType));
            }

            if (!string.IsNullOrEmpty(paymentMethod) && paymentMethod != "الكل")
            {
                query += " AND PaymentMethod = @PaymentMethod";
                parameters.Add(new SqlParameter("@PaymentMethod", paymentMethod));
            }

            if (deliveryDate.HasValue)
            {
                query += " AND DeliveryDate <= @DeliveryDate";
                parameters.Add(new SqlParameter("@DeliveryDate", deliveryDate.Value));
            }

            if (!string.IsNullOrEmpty(activity) && activity != "الكل")
            {
                query += " AND Activity = @Activity";
                parameters.Add(new SqlParameter("@Activity", activity));
            }

            query += " ORDER BY CreatedAt DESC";

            var result = new List<Property>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters.ToArray());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(MapProperty(reader));
            }
            return result;
        }

        public async Task<List<Property>> GetAllAsync()
        {
            return await SearchAsync(null, null, null, null, null, null);
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Properties WHERE PropertyID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapProperty(reader);
            }
            return null;
        }

        public async Task AddAsync(Property property)
        {
            const string query = @"
                INSERT INTO Properties (Title, Description, PropertyType, Activity, Price, PaymentMethod, DeliveryDate, Location, ImageUrl, IsFeatured)
                VALUES (@Title, @Description, @PropertyType, @Activity, @Price, @PaymentMethod, @DeliveryDate, @Location, @ImageUrl, @IsFeatured)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Title", property.Title);
            command.Parameters.AddWithValue("@Description", (object?)property.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@PropertyType", property.PropertyType);
            command.Parameters.AddWithValue("@Activity", property.Activity);
            command.Parameters.AddWithValue("@Price", property.Price);
            command.Parameters.AddWithValue("@PaymentMethod", (object?)property.PaymentMethod ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveryDate", (object?)property.DeliveryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)property.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImageUrl", (object?)property.ImageUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsFeatured", property.IsFeatured);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Property property)
        {
            const string query = @"
                UPDATE Properties 
                SET Title = @Title, Description = @Description, PropertyType = @PropertyType, 
                    Activity = @Activity, Price = @Price, PaymentMethod = @PaymentMethod, 
                    DeliveryDate = @DeliveryDate, Location = @Location, ImageUrl = @ImageUrl,
                    IsAvailable = @IsAvailable, IsFeatured = @IsFeatured
                WHERE PropertyID = @PropertyID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Title", property.Title);
            command.Parameters.AddWithValue("@Description", (object?)property.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@PropertyType", property.PropertyType);
            command.Parameters.AddWithValue("@Activity", property.Activity);
            command.Parameters.AddWithValue("@Price", property.Price);
            command.Parameters.AddWithValue("@PaymentMethod", (object?)property.PaymentMethod ?? DBNull.Value);
            command.Parameters.AddWithValue("@DeliveryDate", (object?)property.DeliveryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)property.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImageUrl", (object?)property.ImageUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsAvailable", property.IsAvailable);
            command.Parameters.AddWithValue("@IsFeatured", property.IsFeatured);
            command.Parameters.AddWithValue("@PropertyID", property.PropertyID);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Properties WHERE PropertyID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Property>> GetFeaturedAsync()
        {
            const string query = @"
                SELECT * FROM Properties 
                WHERE IsAvailable = 1 AND IsFeatured = 1 
                ORDER BY CreatedAt DESC";

            var result = new List<Property>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(MapProperty(reader));
            }
            return result;
        }

        // Helper لتحويل الـ SqlDataReader لـ Property
        private static Property MapProperty(SqlDataReader reader)
        {
            return new Property
            {
                PropertyID = reader.GetInt32(reader.GetOrdinal("PropertyID")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                PropertyType = reader.GetString(reader.GetOrdinal("PropertyType")),
                Activity = reader.GetString(reader.GetOrdinal("Activity")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                PaymentMethod = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? null : reader.GetString(reader.GetOrdinal("PaymentMethod")),
                DeliveryDate = reader.IsDBNull(reader.GetOrdinal("DeliveryDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DeliveryDate")),
                Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                IsAvailable = reader.IsDBNull(reader.GetOrdinal("IsAvailable")) ? false : reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                IsFeatured = reader.IsDBNull(reader.GetOrdinal("IsFeatured")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFeatured")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            };
        }
    }
}