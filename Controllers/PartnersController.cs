

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{

    [Authorize]
    public class PartnersController : Controller
    {
        private readonly PartnersRepository _repository;

        public PartnersController(PartnersRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var partners = await _repository.GetPartnersAsync();
            return View(partners);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new Partner { JoinDate = DateTime.Today });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Partner partner)
        {
            await _repository.AddPartnerAsync(partner);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id)
        {
            var partner = await _repository.GetPartnerByIdAsync(id);
            if (partner == null) return NotFound();
            return View(partner);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Partner partner)
        {
            await _repository.UpdatePartnerAsync(partner);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeletePartnerAsync(id);
            return RedirectToAction("Index");
        }
    }
}