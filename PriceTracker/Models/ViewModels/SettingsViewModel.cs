using System.ComponentModel.DataAnnotations;

namespace PriceTracker.Models.ViewModels
{
    public class SettingsViewModel
    {

        [Display(Name = "Notification Email")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [Required(ErrorMessage = "Email is required")]
        public string NotificationEmail { get; set; } = string.Empty;
        public string SelectedTheme { get; set; } = "light"; // default
    }
}
