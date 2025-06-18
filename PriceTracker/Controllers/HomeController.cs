using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ITagService _tagService;

        public HomeController(IProductService productService, ITagService tagService)
        {
            _productService = productService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            var allProducts = await _productService.GetAllProductsAsync() ?? new List<ProductDto>();
            vm.TopThreeProducts = allProducts
                .OrderBy(p => p.ProductId)
                .Take(3)
                .ToList();

            var allTags = await _tagService.GetAllAsync() ?? new List<TagDto>();
            vm.TopThreeTags = allTags
                .OrderBy(t => t.TagId)
                .Take(3)
                .ToList();

            return View(vm);
        }
    }
}
