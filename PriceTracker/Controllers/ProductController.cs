using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models.ViewModels;
using PriceTracker.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using PriceTracker.Services;

namespace PriceTracker.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly IPriceEntryService _priceEntryService;
        private readonly IEmailService _emailService;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IPriceEntryService priceEntryService, IEmailService emailService)
        {
            _productService = productService;
            _priceEntryService = priceEntryService;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new ProductViewModel();
            var products = await _productService.GetAllProductsAsync();
            vm.Products = products
                ?.OrderByDescending(p => p.CreatedAt)
                .ToList();
            return View(vm);
        }


        [HttpGet]
        public IActionResult New()
        {
            return View(new NewProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewProductViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var productDto = new CreateProductDto
            {
                Name = vm.Name,
                Url = vm.Url,
                Description = vm.Description,
                CompanyName = vm.CompanyName,
                CurrentPrice = vm.LatestPrice,
                PriceSource = vm.LatestPriceSource,
                CategoryNames = vm.CategoryNames,
                Tags = vm.Tags
            };


            var response = await _productService.CreateProductAsync(productDto);

            if (response.Status == ServiceResponse<ProductDto>.ServiceStatus.Created)
            {
                if (!string.IsNullOrWhiteSpace(SettingsController.NotificationEmail))
                {
                    await EmailSender.SendEmailAsync(
                        SettingsController.NotificationEmail,
                        "New Product Created",
                        $"Product <strong>{productDto.Name}</strong> has been created."
                    );
                }

                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("Index");
            }

            TempData["FailMessage"] = response.Messages.Count > 0
                ? string.Join("; ", response.Messages)
                : "There was an error creating the product.";

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var vm = new NewProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Url = product.Url,
                Description = product.Description,
                CompanyName = product.CompanyName,
                LatestPrice = product.LatestPrice??0,
                LatestPriceSource = product.LatestPriceSource,
                CategoryNames = product.Categories,
                Tags = product.Tags
            };
            return View("New", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewProductViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("New", vm);
            }

            var productDto = new CreateProductDto
            {
                Name = vm.Name,
                Url = vm.Url,
                Description = vm.Description,
                CompanyName = vm.CompanyName,
                CurrentPrice = vm.LatestPrice,
                PriceSource = vm.LatestPriceSource,
                CategoryNames = vm.CategoryNames,
                Tags = vm.Tags
            };

            var result = await _productService.UpdateProductAsync(id, productDto);

            if (result.Status == ServiceResponse<ProductDto>.ServiceStatus.Updated)
            {
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["FailMessage"] = result.Messages.Count > 0
                ? string.Join("; ", result.Messages)
                : "Failed to update product.";

            return View("New", vm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePrice(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found.");

            var priceEntries = await _priceEntryService.GetByProductIdAsync(id);

            var history = priceEntries
                .OrderBy(pe => pe.RecordedAt)
                .Select(pe => new PricePoint
                {
                    Price = pe.Price,
                    RecordedAt = pe.RecordedAt
                })
                .ToList();

            var vm = new PriceHistoryViewModel
            {
                ProductId = id,
                ProductName = product.Name,
                PriceHistory = history
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrice(PriceHistoryViewModel vm)
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
                        Price = pe.Price,
                        RecordedAt = pe.RecordedAt
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
            if (created.Status != ServiceResponse<PriceEntryDto>.ServiceStatus.Created)
            {
                var errorMsg = created.Messages.Count > 0 ? string.Join(", ", created.Messages) : "Failed to add new price entry.";
                ModelState.AddModelError(string.Empty, errorMsg);

                // Refill PriceHistory for the view if error occurs
                var priceEntries = await _priceEntryService.GetByProductIdAsync(vm.ProductId);
                vm.PriceHistory = priceEntries
                    .OrderBy(pe => pe.RecordedAt)
                    .Select(pe => new PricePoint
                    {
                        Price = pe.Price,
                        RecordedAt = pe.RecordedAt
                    })
                    .ToList();

                return View(vm);
            }

            return RedirectToAction(nameof(UpdatePrice), new { productId = vm.ProductId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteConfirm", product); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (result.Status == ServiceResponse<bool>.ServiceStatus.Deleted)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            else
            {
                TempData["FailMessage"] = result.Messages.Count > 0
                    ? string.Join("; ", result.Messages)
                    : "Failed to delete product.";
            }

            return RedirectToAction("Index");
        }


    }
}
