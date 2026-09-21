
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;  // ← السطر الجديد ده

using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{
    [Authorize]
    public class LedgerController : Controller
    {
        private readonly LedgerRepository _repository;

        public LedgerController(LedgerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new LedgerViewModel();

            try
            {
                model.Lines = await _repository.GetLedgerLinesAsync();
                model.TrialBalance = await _repository.GetTrialBalanceAsync();
                model.BalanceCheck = await _repository.GetBalanceCheckAsync();
            }
            catch (Exception ex)
            {
                // لو قاعدة البيانات مش شغالة أو الـ Connection String غلط
                // هيظهر رسالة واضحة في الصفحة بدل ما البرنامج يقع
                model.ErrorMessage = "تعذر الاتصال بقاعدة البيانات: " + ex.Message +
                    " — تأكد من تشغيل SQL Server ومن صحة الـ Connection String في appsettings.json";
            }

            return View(model);
        }

        // ============ Actions الإضافة والتعديل والحذف ============

        // عرض شاشة إضافة قيد

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Accounts = await _repository.GetAccountsAsync();
            return View(new JournalEntry
            {
                EntryNumber = "J-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                TransactionDate = DateTime.Today,
                Lines = new List<JournalLine>
        {
            new JournalLine { Debit = 0, Credit = 0 },
            new JournalLine { Debit = 0, Credit = 0 }
        }
            });
    }

        // حفظ القيد الجديد
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JournalEntry entry)
        {
            try
            {
                await _repository.AddJournalEntryAsync(entry);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Accounts = await _repository.GetAccountsAsync();
                ViewBag.ErrorMessage = ex.Message;
                return View(entry);
            }
        }

        // عرض شاشة تعديل قيد

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string entryNumber)
        {
            var entry = await _repository.GetJournalEntryByNumberAsync(entryNumber);
            if (entry == null) return NotFound();

            ViewBag.Accounts = await _repository.GetAccountsAsync();
            return View(entry);
        }

        // حفظ التعديل

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string oldEntryNumber, JournalEntry entry)
        {
            try
            {
                await _repository.UpdateJournalEntryAsync(oldEntryNumber, entry);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Accounts = await _repository.GetAccountsAsync();
                ViewBag.ErrorMessage = ex.Message;
                return View(entry);
            }
        }

        // حذف قيد

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string entryNumber)
        {
            await _repository.DeleteJournalEntryAsync(entryNumber);
            return RedirectToAction(nameof(Index));
        }
    }
}
