using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;

namespace PriceTracker.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductsAPIController _productsApi;
        private readonly PriceEntryAPIController _priceEntriesApi;

        public ProductController(ProductsAPIController productsApi, PriceEntryAPIController priceEntriesApi)
        {
            _productsApi = productsApi;
            _priceEntriesApi = priceEntriesApi;
        }


        [HttpGet]
        public IActionResult New()
        {
            var vm = new NewProductViewModel();
            return View(vm);
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

            var apiResponse = await _productsApi.CreateProduct(productDto);
            Console.WriteLine(apiResponse.ToString());
            if (apiResponse is OkResult || apiResponse is OkObjectResult || apiResponse is CreatedResult)
            {
                Console.WriteLine("here");
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("Index");
            }


            TempData["FailMessage"] = "There was an error creating the product.";
            return View(vm);
        }
    }
}
