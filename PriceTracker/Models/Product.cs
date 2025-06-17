 using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models
{

    public enum ProductCondition
    {
        New,
        Used,
        Refurbished,
        Damaged
    }

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Url { get; set; }

        [EnumDataType(typeof(ProductCondition), ErrorMessage = "Invalid condition selected.")]
        public string? Condition { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        // One Product → Many PriceEntries
        public ICollection<PriceEntry>? PriceEntries { get; set; }

        // Many-to-Many with Category via ProductCategory
        public ICollection<ProductCategory>? ProductCategories { get; set; }

        public ICollection<ProductTag>? ProductTags { get; set; }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        // Most recent price info
        public decimal? LatestPrice { get; set; }
        public DateTime? LatestPriceRecordedAt { get; set; }
        public string? LatestPriceSource { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal CurrentPrice { get; set; }

        [Required]
        [StringLength(100)]
        public string PriceSource { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> CategoryNames { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Tags { get; set; }

        public string? TagSource { get; set; }
    }


}
