using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models
{
    public class Company
    {
        [Key]
        public int ComapnyId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Website { get; set; }

        [Required]
        public string ContactEmail { get; set; }

        // One Company → Many Products
        public ICollection<Product>? Products { get; set; }


    }
}
