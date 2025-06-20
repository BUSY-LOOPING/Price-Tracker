using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models.ViewModels;

namespace PriceTracker.Controllers
{
    public class SettingsController : Controller
    {
        public static string NotificationEmail = "";

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new SettingsViewModel
            {
                NotificationEmail = NotificationEmail
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(SettingsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            NotificationEmail = vm.NotificationEmail; 
            TempData["SuccessMessage"] = "Settings saved successfully.";
            return RedirectToAction("Index");
        }
    }
}
