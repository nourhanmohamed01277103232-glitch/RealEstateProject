using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{

    [Authorize]
    public class CapitalMovementsController : Controller
    {
        private readonly PartnersRepository _repository;

        public CapitalMovementsController(PartnersRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var movements = await _repository.GetMovementsAsync();
            return View(movements);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Partners = await _repository.GetPartnersAsync();
            return View(new CapitalMovement { MovementDate = DateTime.Today });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CapitalMovement movement)
        {
            await _repository.AddMovementAsync(movement);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteMovementAsync(id);
            return RedirectToAction("Index");
        }
    }
}