using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{
    public class BookingController : Controller
    {
        private readonly BookingRepository _repository;
        private readonly PropertiesRepository _propertiesRepo;

        public BookingController(BookingRepository repository, PropertiesRepository propertiesRepo)
        {
            _repository = repository;
            _propertiesRepo = propertiesRepo;
        }

        // عرض صفحة الحجز
        public async Task<IActionResult> Create(int? propertyId)
        {
            if (propertyId.HasValue)
            {
                var property = await _propertiesRepo.GetByIdAsync(propertyId.Value);
                if (property != null)
                {
                    ViewBag.PropertyTitle = property.Title;
                    ViewBag.PropertyType = property.PropertyType;
                    ViewBag.PropertyPrice = property.Price;

                    return View(new Booking
                    {
                        UnitType = property.PropertyType,
                        PropertyID = property.PropertyID
                    });
                }
            }

            return View(new Booking());
        }

        // حفظ الحجز
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddBookingAsync(booking);
                ViewBag.Success = "تم تسجيل حجزك بنجاح! هنتواصل معاك قريباً.";
                return View(new Booking());
            }
            return View(booking);
        }
    }
}