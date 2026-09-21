# مشروع عقارات — الموقع الكامل

## 🧩 محتوى المشروع
- **الصفحة الرئيسية** (بحث عن عقار + قسم المشاريع الجديدة + بانر "استلم وحدتك")
- **صفحة دفتر الأستاذ** (متصلة بقاعدة بيانات SQL Server الحقيقية)
- تصميم متجاوب (Responsive) بالكامل، وباللغة العربية RTL

## ▶️ خطوات التشغيل على جهازك (Visual Studio)

1. فك ضغط المجلد، وافتح ملف `RealEstateProject.csproj` بالدبل كليك — هيفتح مباشرة في Visual Studio.
2. افتح ملف `Database/ledger.sql` في **SQL Server Management Studio** ونفّذه بالكامل (Execute) — هيعمل لك قاعدة بيانات `AccountingDB` بكل الجداول والبيانات التجريبية.
3. افتح ملف `appsettings.json` وتأكد إن سطر الاتصال ده مطابق لاسم السيرفر بتاعك:
   ```
   "AccountingDB": "Server=.\\SQLEXPRESS;Database=AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
   لو اسم السيرفر عندك مختلف (شايفه في SSMS جنب Connect)، غيّر `.\SQLEXPRESS` بالاسم الصحيح.
4. اضغط **F5** أو زرار **Run** في Visual Studio — الموقع هيفتح في المتصفح تلقائي.
5. من الناف بار روح على "دفتر الأستاذ" وهتلاقي بيانات القيود وميزان المراجعة ظاهرة لايف من القاعدة.

## 🖼️ فين أحط الصور والبيانات اللي لسه ناقصة؟

| العنصر | مكانه بالظبط |
|---|---|
| **صورة خلفية الهيرو** (بديل صورة القطار) | حط الصورة في `wwwroot/images/hero-bg.jpg`، وبعدين افتح `wwwroot/css/site.css` وشيل التعليق `/* */` عن سطر `background-image` جوه `.hero-section` |
| **خلفية الصفحة البيضاء** | لو عايز صورة بدل اللون الأبيض، حط الصورة في `wwwroot/images/page-bg.jpg` وشيل التعليق عن سطر `background-image` جوه `body` في نفس ملف الـ CSS |
| **رقم الواتساب** | فيه 4 أماكن مكتوب فوقيهم `TODO` في `Views/Shared/_Layout.cshtml` و `Views/Home/Index.cshtml` — دور على `wa.me/201234567890` وبدّل الرقم برقمك (بصيغة دولية بدون + وبدون أصفار في الأول، مثال رقم مصري: 201001234567) |
| **اللوجو** | في `Views/Shared/_Layout.cshtml` تحت تعليق `TODO: هنا مكان اللوجو` — حط صورة اللوجو في `wwwroot/images/logo.png` واستخدم السطر الجاهز المكتوب كمثال جنب التعليق |
| **اسم الشركة** | ابحث عن كلمة "عقارات" في `_Layout.cshtml` (فوق وتحت) وبدّلها باسم شركتك |
| **رقم التليفون والإيميل** | في الفوتر بـ `_Layout.cshtml`، دور على `01234567010` و `info@realestate.com` |

كل مكان من دول متعلّم عليه بتعليق `TODO` جوه الكود عشان تلاقيه بسهولة.

## 📁 هيكل المشروع
```
RealEstateProject/
├── Controllers/
│   ├── HomeController.cs      → الصفحة الرئيسية
│   └── LedgerController.cs    → صفحة دفتر الأستاذ
├── Models/
│   └── LedgerModels.cs        → أشكال بيانات دفتر الأستاذ
├── Data/
│   └── LedgerRepository.cs    → الاتصال بقاعدة البيانات وجلب البيانات
├── Views/
│   ├── Home/Index.cshtml      → صفحة البحث والمشاريع
│   ├── Ledger/Index.cshtml    → صفحة دفتر الأستاذ
│   └── Shared/_Layout.cshtml  → الهيدر والفوتر المشتركين
├── Database/ledger.sql        → سكريبت إنشاء قاعدة البيانات
├── wwwroot/css/site.css       → كل التنسيقات والألوان
└── appsettings.json           → إعدادات الاتصال بقاعدة البيانات
```

## ⚠️ لو صفحة دفتر الأستاذ ظهرت فيها رسالة خطأ
غالبًا يبقى السبب:
- SQL Server مش شغال، أو
- الـ Connection String في `appsettings.json` مش مطابق لاسم السيرفر عندك

الرسالة اللي هتظهر في الصفحة نفسها هتقولك التفاصيل بالظبط.
