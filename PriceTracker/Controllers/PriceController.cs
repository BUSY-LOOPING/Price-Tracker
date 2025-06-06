// Controllers/PriceController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    public class PriceController : Controller
    {
        private readonly ProductsAPIController _productsApi;
        private readonly PriceEntryAPIController _priceEntriesApi;

        public PriceController(
            ProductsAPIController productsApi,
            PriceEntryAPIController priceEntriesApi)
        {
            _productsApi = productsApi;
            _priceEntriesApi = priceEntriesApi;
        }

        // GET: /Price/Update/{productId}
        [HttpGet]
        public async Task<IActionResult> Update(int productId)
        {
            var prodResponse = await _productsApi.GetProducts();
            IEnumerable<ProductDto> allProducts = null;

            if (prodResponse.Value != null)
            {
                allProducts = prodResponse.Value;
            }
            else if (prodResponse.Result is OkObjectResult okProd)
            {
                allProducts = okProd.Value as IEnumerable<ProductDto>;
            }

            allProducts ??= new List<ProductDto>();
            var product = allProducts.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                return NotFound($"Product with ID {productId} not found.");

            var priceEntriesResponse = await _priceEntriesApi.GetPriceEntriesForProduct(productId);
            IEnumerable<PriceEntryDto> entries = null;

            if (priceEntriesResponse.Value != null)
            {
                entries = priceEntriesResponse.Value;
            }
            else if (priceEntriesResponse.Result is OkObjectResult okEntries)
            {
                entries = okEntries.Value as IEnumerable<PriceEntryDto>;
            }

            entries ??= new List<PriceEntryDto>();

            var history = entries
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
                // NewRecordedAt defaults to DateTime.Now
            };

            return View(vm);
        }

        // POST: /Price/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PriceHistoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Reload product name and history so the form can re-render
                var prodResponse = await _productsApi.GetProducts();
                IEnumerable<ProductDto> allProducts = null;

                if (prodResponse.Value != null)
                    allProducts = prodResponse.Value;
                else if (prodResponse.Result is OkObjectResult okProd)
                    allProducts = okProd.Value as IEnumerable<ProductDto>;

                allProducts ??= new List<ProductDto>();
                var product = allProducts.FirstOrDefault(p => p.ProductId == vm.ProductId);
                if (product == null)
                    return NotFound($"Product with ID {vm.ProductId} not found.");

                vm.ProductName = product.Name;

                var priceEntriesResponse = await _priceEntriesApi.GetPriceEntriesForProduct(vm.ProductId);
                IEnumerable<PriceEntryDto> entries = null;
                if (priceEntriesResponse.Value != null)
                    entries = priceEntriesResponse.Value;
                else if (priceEntriesResponse.Result is OkObjectResult okEntries)
                    entries = okEntries.Value as IEnumerable<PriceEntryDto>;

                entries ??= new List<PriceEntryDto>();
                vm.PriceHistory = entries
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
            };

            
            var createResponse = await _priceEntriesApi.CreatePriceEntry(newDto);
            if (createResponse is BadRequestObjectResult badReq)
            {
                ModelState.AddModelError(string.Empty, "Failed to add new price entry.");
                return View(vm);
            }

            return RedirectToAction(nameof(Update), new { productId = vm.ProductId });
        }
    }
}
