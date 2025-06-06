using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models.ViewModels
{
    public class PriceHistoryViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public List<PricePoint> PriceHistory { get; set; } = new();

        [Display(Name = "New Price")]
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal NewPrice { get; set; }

        [Display(Name = "Source")]
        [Required(ErrorMessage = "Source is required")]
        [StringLength(100)]
        public string NewPriceSource { get; set; } = string.Empty;
    }

    public class PricePoint
    {
        public DateTime RecordedAt { get; set; }
        public decimal Price { get; set; }
    }
}
