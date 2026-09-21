using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{
   
    public class PropertiesController : Controller
    {
        private readonly PropertiesRepository _repository;

        public PropertiesController(PropertiesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var properties = await _repository.GetAllAsync();
            return View(properties);
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _repository.GetByIdAsync(id);
            if (property == null) return NotFound();
            return View(property);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Property property)
        {
            if (ModelState.IsValid)
            {
                if (property.ImageFile != null && property.ImageFile.Length > 0)
                {
                    property.ImageUrl = await SaveImageAsync(property.ImageFile);
                }

                await _repository.AddAsync(property);
                return RedirectToAction(nameof(Index));
            }
            return View(property);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _repository.GetByIdAsync(id);
            if (property == null) return NotFound();
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.PropertyID) return NotFound();

            if (ModelState.IsValid)
            {
                if (property.ImageFile != null && property.ImageFile.Length > 0)
                {
                    property.ImageUrl = await SaveImageAsync(property.ImageFile);
                }

                await _repository.UpdateAsync(property);
                return RedirectToAction(nameof(Index));
            }
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "units");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/units/" + uniqueFileName;
        }
    }
}