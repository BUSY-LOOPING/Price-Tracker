using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models.ViewModels
{
    public class NewTagViewModel 
    {
        public int? TagId { get; set; }
        [MaxLength(20, ErrorMessage = "Name is too long")]
        [MinLength(2, ErrorMessage = "Name is too short")]
        [Required(ErrorMessage = "Tag Name is required")]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}
