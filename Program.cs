using Microsoft.AspNetCore.Authentication.Cookies;
using RealEstateProject.Data;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمة الـ MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// ============ إعدادات المصادقة (Cookie Authentication) ============
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });

// تسجيل خدمة الوصول للبيانات
builder.Services.AddScoped<RealEstateProject.Data.LedgerRepository>();
builder.Services.AddScoped<RealEstateProject.Data.PartnersRepository>();
builder.Services.AddScoped<ProjectsRepository>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<PropertiesRepository>();  // ← السطر الجديد للبحث

var app = builder.Build();

// إعدادات بيئة التشغيل
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ============ ترتيب مهم جداً ============
app.UseAuthentication();  // 1. التحقق من الهوية
app.UseAuthorization();   // 2. التحقق من الصلاحيات

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();