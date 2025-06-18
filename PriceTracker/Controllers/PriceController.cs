using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;
using PriceTracker.Services;

namespace PriceTracker.Controllers
{
    public class PriceController : Controller
    {
        private readonly IProductService _productService;
        private readonly IPriceEntryService _priceEntryService;

        public PriceController(
            IProductService productService,
            IPriceEntryService priceEntryService)
        {
            _productService = productService;
            _priceEntryService = priceEntryService;
        }

        [HttpGet]
        public async Task<IActionResult> Update(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
                return NotFound($"Product with ID {productId} not found.");

            var priceEntries = await _priceEntryService.GetByProductIdAsync(productId);

            var history = priceEntries
                .OrderBy(pe => pe.RecordedAt)
                .Select(pe => new PricePoint
                {
                    Price = pe.Price
                })
                .ToList();

            var vm = new PriceHistoryViewModel
            {
                ProductId = productId,
                ProductName = product.Name,
                PriceHistory = history
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PriceHistoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var product = await _productService.GetByIdAsync(vm.ProductId);
                if (product == null)
                    return NotFound($"Product with ID {vm.ProductId} not found.");

                vm.ProductName = product.Name;

                var priceEntries = await _priceEntryService.GetByProductIdAsync(vm.ProductId);
                vm.PriceHistory = priceEntries
                    .OrderBy(pe => pe.RecordedAt)
                    .Select(pe => new PricePoint
                    {
                        Price = pe.Price
                    })
                    .ToList();

                return View(vm);
            }

            var newDto = new PriceEntryDto
            {
                ProductId = vm.ProductId,
                Price = vm.NewPrice,
                Source = vm.NewPriceSource,
                RecordedAt = DateTime.UtcNow
            };

            var created = await _priceEntryService.CreateAsync(newDto);
            if(created.Status != ServiceResponse<PriceEntryDto>.ServiceStatus.Created)
            {
                var errorMsg = created.Messages.Count > 0 ? string.Join(", ", created.Messages) : "Failed to add new price entry.";
                ModelState.AddModelError(string.Empty, errorMsg);
                return View(vm);
            }
            return RedirectToAction(nameof(Update), new { productId = vm.ProductId });
        }
    }
}
