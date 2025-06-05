using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    public class PriceEntry
    {
        [Key]
        public int EntryId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Precision(18, 4)] // Specifies precision and scale: if we dont add this , it will give us warning
                           // This allows us total of 18 digits, with 4 digits after the decimal point
        public decimal Price { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public DateTime RecordedAt { get; set; }
    }

    public class PriceEntryDto
    {
        [Required]
        public int EntryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public DateTime RecordedAt { get; set; }
    }
}
