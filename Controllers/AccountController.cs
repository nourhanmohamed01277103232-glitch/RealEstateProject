using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RealEstateProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // صفحة تسجيل الدخول (GET)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // تسجيل الدخول (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string password, string? returnUrl = null)
        {
            var companyPassword = _configuration["AuthSettings:CompanyPassword"];
            var adminPassword = _configuration["AuthSettings:AdminPassword"];
            var companyRole = _configuration["AuthSettings:CompanyRole"] ?? "Company";
            var adminRole = _configuration["AuthSettings:AdminRole"] ?? "Admin";

           
            string? role = null;

            // التحقق من الباسورد
            if (password == adminPassword)
                role = adminRole;
            else if (password == companyPassword)
                role = companyRole;

            // لو الباسورد غلط
            if (role == null)
            {
                ViewBag.Error = "كلمة السر غير صحيحة";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // إنشاء الهوية (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, role == adminRole ? "الأدمن" : "الشركة"),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // تسجيل الخروج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // صفحة "مش عندك صلاحية"
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}