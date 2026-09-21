using Microsoft.Data.SqlClient;
using RealEstateProject.Models;

namespace RealEstateProject.Data
{
    public class BookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PartnersDB")
                ?? throw new InvalidOperationException("لم يتم العثور على Connection String");
        }

        // إضافة حجز جديد
        public async Task AddBookingAsync(Booking booking)
        {
            const string query = @"
                INSERT INTO Bookings (FullName, Phone, Email, UnitType, PropertyID, DownPayment, Notes, BookingDate)
                VALUES (@FullName, @Phone, @Email, @UnitType, @PropertyID, @DownPayment, @Notes, GETDATE())";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FullName", booking.FullName);
            command.Parameters.AddWithValue("@Phone", booking.Phone);
            command.Parameters.AddWithValue("@Email", (object?)booking.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnitType", (object?)booking.UnitType ?? DBNull.Value);
            command.Parameters.AddWithValue("@PropertyID", (object?)booking.PropertyID ?? DBNull.Value);
            command.Parameters.AddWithValue("@DownPayment", (object?)booking.DownPayment ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)booking.Notes ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // جيب كل الحجوزات (مع اسم الوحدة)
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            const string query = @"
                SELECT b.BookingID, b.FullName, b.Phone, b.Email, b.UnitType, 
                       b.PropertyID, p.Title AS PropertyTitle,
                       b.DownPayment, b.Notes, b.BookingDate
                FROM Bookings b
                LEFT JOIN Properties p ON b.PropertyID = p.PropertyID
                ORDER BY b.BookingDate DESC";

            var result = new List<Booking>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new Booking
                {
                    BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                    UnitType = reader.IsDBNull(reader.GetOrdinal("UnitType")) ? null : reader.GetString(reader.GetOrdinal("UnitType")),
                    PropertyID = reader.IsDBNull(reader.GetOrdinal("PropertyID")) ? null : reader.GetInt32(reader.GetOrdinal("PropertyID")),
                    PropertyTitle = reader.IsDBNull(reader.GetOrdinal("PropertyTitle")) ? null : reader.GetString(reader.GetOrdinal("PropertyTitle")),
                    DownPayment = reader.IsDBNull(reader.GetOrdinal("DownPayment")) ? null : reader.GetDecimal(reader.GetOrdinal("DownPayment")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                });
            }
            return result;
        }

        // حذف حجز
        public async Task DeleteBookingAsync(int id)
        {
            const string query = "DELETE FROM Bookings WHERE BookingID = @id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}