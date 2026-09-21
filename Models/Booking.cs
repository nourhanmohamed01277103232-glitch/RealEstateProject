using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "الاسم لازم يكون بين 3 و 200 حرف")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "رقم التليفون مطلوب")]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "رقم التليفون لازم يكون 11 رقم ويبدأ بـ 01")]
        [Display(Name = "رقم التليفون")]
        public string Phone { get; set; } = "";

        [EmailAddress(ErrorMessage = "الإيميل غير صحيح")]
        [Display(Name = "الإيميل")]
        public string? Email { get; set; }

        [Display(Name = "نوع الوحدة")]
        public string? UnitType { get; set; }

        // ← جديد: ربط الحجز بالوحدة
        [Display(Name = "الوحدة")]
        public int? PropertyID { get; set; }

        // للعرض فقط (مش بيتخزن في قاعدة البيانات)
        public string? PropertyTitle { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "المبلغ المقدم لازم يكون أكبر من أو يساوي صفر")]
        [Display(Name = "المبلغ المقدم")]
        public decimal? DownPayment { get; set; }

        [StringLength(500, ErrorMessage = "الملاحظات ما تزيدش عن 500 حرف")]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public DateTime BookingDate { get; set; }
    }
}