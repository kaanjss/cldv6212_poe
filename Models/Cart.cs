using System.ComponentModel.DataAnnotations;

namespace ABCRetailers.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string ProductId { get; set; } = string.Empty; // Azure Table Storage GUID

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice => Quantity * UnitPrice;

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        // Navigation properties (not stored in DB)
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}

