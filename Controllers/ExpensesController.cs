using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{

    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly PartnersRepository _repository;

        public ExpensesController(PartnersRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _repository.GetExpensesAsync();
            return View(expenses);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CompanyExpense { ExpenseDate = DateTime.Today });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CompanyExpense expense)
        {
            await _repository.AddExpenseAsync(expense);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var shares = await _repository.GetExpenseSharesAsync(id);
            return View(shares);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteExpenseAsync(id);
            return RedirectToAction("Index");
        }
    }
}