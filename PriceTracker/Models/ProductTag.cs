using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PriceTracker.Models
{
    [PrimaryKey(nameof(ProductId), nameof(TagId))]
    public class ProductTag
    {
        //[Key, Column(Order = 0)]
        [Required]

        public int ProductId { get; set; }

        //[Key, Column(Order = 1)]
        [Required]
        public int TagId { get; set; }

        public string? TagSource { get; set; } // "User", "Admin", "Automated"

        public virtual Product Product { get; set; }

        public virtual Tag Tag { get; set; }


    }

    public class TagDto
    {
        [Required]
        public int TagId { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}
