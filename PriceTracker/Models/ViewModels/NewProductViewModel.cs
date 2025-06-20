using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models.ViewModels
{
    public class NewProductViewModel 
    {
        public int? ProductId { get; set; }
        bool hasData = false;
        [MaxLength(50, ErrorMessage = "Name is too long")]
        [MinLength(2, ErrorMessage = "Name is too short")]
        [Required(ErrorMessage = "Price is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "URL is required")]

        public string Url { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Latest Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal LatestPrice { get; set; }

        [Required(ErrorMessage = "Price Source is required")]
        public string LatestPriceSource { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> CategoryNames { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Tags { get; set; }

    }
}
