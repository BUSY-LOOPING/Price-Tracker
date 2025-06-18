using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    [PrimaryKey(nameof(ProductId), nameof(CategoryId))]
    public class ProductCategory
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Category Category { get; set; }

    }

}
