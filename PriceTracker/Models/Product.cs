using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public required virtual Company Company { get; set; }

        // One Product → Many PriceEntries
        public ICollection<PriceEntry>? PriceEntries { get; set; }

        // Many-to-Many with Category via ProductCategory
        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}
