using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;

namespace RealEstateProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly PropertiesRepository _propertiesRepo;

        public HomeController(PropertiesRepository propertiesRepo)
        {
            _propertiesRepo = propertiesRepo;
        }

        public async Task<IActionResult> Index(
            decimal? priceFrom, decimal? priceTo,
            string? propertyType, string? paymentMethod,
            DateTime? deliveryDate, string? activity)
        {
            var properties = await _propertiesRepo.SearchAsync(
                priceFrom, priceTo, propertyType, paymentMethod, deliveryDate, activity);

            var featured = await _propertiesRepo.GetFeaturedAsync();

            ViewBag.FeaturedProperties = featured;
            ViewBag.PriceFrom = priceFrom;
            ViewBag.PriceTo = priceTo;
            ViewBag.PropertyType = propertyType;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.DeliveryDate = deliveryDate;
            ViewBag.Activity = activity;

            bool hasFilter =
                priceFrom.HasValue ||
                priceTo.HasValue ||
                (!string.IsNullOrEmpty(propertyType) && propertyType != "الكل") ||
                (!string.IsNullOrEmpty(paymentMethod) && paymentMethod != "الكل") ||
                deliveryDate.HasValue ||
                (!string.IsNullOrEmpty(activity) && activity != "الكل");

            ViewBag.ScrollToProjects = hasFilter;

            return View(properties);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}