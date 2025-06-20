using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models.ViewModels
{
    public class NewCategoryViewModel 
    {
        public int? CategoryId { get; set; }
        [MaxLength(20, ErrorMessage = "Name is too long")]
        [MinLength(2, ErrorMessage = "Name is too short")]
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
