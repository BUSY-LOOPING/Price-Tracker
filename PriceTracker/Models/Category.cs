using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Many-to-Many with Product via ProductCategory
        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public required string Name { get; set; }
    }

    public class CreateCategoryDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
