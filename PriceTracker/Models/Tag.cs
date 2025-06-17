using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int PopularityScore { get; set; } = 0; // Useful for trending searches

        public bool IsActive { get; set; } = true; // If tag is currently used: for soft deletes

        // Many-to-Many with Product via ProductTag
        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}
