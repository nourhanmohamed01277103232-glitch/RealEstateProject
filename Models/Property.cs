using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class Property
    {
        public int PropertyID { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string PropertyType { get; set; } = "";
        public string Activity { get; set; } = "";
        public decimal Price { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        [Display(Name = "وحدة مميزة")]
        public bool IsFeatured { get; set; }
    }
}