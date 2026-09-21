using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;

namespace RealEstateProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly BookingRepository _repository;

        public BookingsController(BookingRepository repository)
        {
            _repository = repository;
        }

        // عرض كل الحجوزات
        public async Task<IActionResult> Index()
        {
            var bookings = await _repository.GetAllBookingsAsync();
            return View(bookings);
        }

        // حذف حجز
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteBookingAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}