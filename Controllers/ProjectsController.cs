
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace RealEstateProject.Controllers
{

    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ProjectsRepository _projectsRepo;

        public ProjectsController(ProjectsRepository projectsRepo)
        {
            _projectsRepo = projectsRepo;
        }

        // عرض كل المشاريع
        public async Task<IActionResult> Index()
        {
            var projects = await _projectsRepo.GetProjectsAsync();
            return View(projects);
        }

        // تفاصيل مشروع
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectsRepo.GetProjectByIdAsync(id);
            if (project == null) return NotFound();

            var installments = await _projectsRepo.GetInstallmentsByProjectAsync(id);
            ViewBag.Installments = installments;
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        // إضافة مشروع - GET
        public IActionResult Create()
        {
            return View();
        }

        // إضافة مشروع - POST

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                await _projectsRepo.AddProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // تعديل مشروع - GET

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectsRepo.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            return View(project);
        }

        // تعديل مشروع - POST

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.ProjectID) return NotFound();

            if (ModelState.IsValid)
            {
                await _projectsRepo.UpdateProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // حذف مشروع

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectsRepo.DeleteProjectAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // إضافة قسط
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInstallment(ProjectInstallment installment)
        {
            if (ModelState.IsValid)
            {
                await _projectsRepo.AddInstallmentAsync(installment);
            }
            return RedirectToAction(nameof(Details), new { id = installment.ProjectID });
        }
    }
}