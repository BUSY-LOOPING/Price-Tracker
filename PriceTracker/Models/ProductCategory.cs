using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    [PrimaryKey(nameof(ProductId), nameof(CategoryId))]
    public class ProductCategory
    {
        //[Key, Column(Order = 0)]
        [Required]
        public int ProductId { get; set; }

        //[Key, Column(Order = 1)]
        [Required]
        public int CategoryId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Category Category { get; set; }


    }
}
