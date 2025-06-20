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
        public ICollection<PriceEntry> PriceEntries { get; set; } = new List<PriceEntry>();

        // Many-to-Many with Category via ProductCategory
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string? Condition { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        // List of related categories (names only)
        public List<string> Categories { get; set; } = new List<string>();

        // List of tags (names only)
        public List<string> Tags { get; set; } = new List<string>();

        // Most recent price entry
        public decimal? LatestPrice { get; set; }
        public string? LatestPriceSource { get; set; }
        public DateTime? LatestPriceRecordedAt { get; set; }

        // Metrics
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? AveragePrice { get; set; }
        public decimal? PriceChangeSinceLast { get; set; }
        public double? PriceChangePercentage { get; set; }
    }


    public class CreateProductDto
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required string Url { get; set; }

        public string? Condition { get; set; }

        public int CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public decimal CurrentPrice { get; set; }

        [Required]
        public required string PriceSource { get; set; }

        [Required]
        public List<string>? CategoryNames { get; set; } = new List<string>();

        [Required]
        public List<string>? Tags { get; set; } = new List<string>();
    }


}
