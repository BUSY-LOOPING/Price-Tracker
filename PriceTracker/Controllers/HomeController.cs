using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ITagService _tagService;
        private readonly ICategoryService _categoryService;
        private readonly IPriceEntryService _priceService;

        public HomeController(
            IProductService productService,
            ITagService tagService,
            ICategoryService categoryService,
            IPriceEntryService priceService)
        {
            _productService = productService;
            _tagService = tagService;
            _categoryService = categoryService;
            _priceService = priceService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            // Load all products
            var products = (await _productService.GetAllProductsAsync()).ToList();
            vm.TotalProducts = products.Count;

            vm.RecentProducts = products
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToList();

            // Load all tags
            var tags = (await _tagService.GetAllAsync()).ToList();
            vm.TotalTags = tags.Count;

            // Load all categories
            var categories = (await _categoryService.GetAllAsync()).ToList();
            vm.TotalCategories = categories.Count;

            // Load all price entries
            var priceEntries = (await _priceService.GetAllAsync()).ToList();

            // Generate price trend data
            vm.PriceTrends = priceEntries
                .OrderBy(pe => pe.RecordedAt)
                .Select(pe => new PriceTrendPoint
                {
                    RecordedAt = pe.RecordedAt,
                    Price = pe.Price,
                    ProductName = products.FirstOrDefault(p => p.ProductId == pe.ProductId)?.Name ?? "Unknown"
                })
                .ToList();

            return View(vm);
        }

        [HttpGet]
        public IActionResult New()
        {
            var model = new NewProductViewModel();
            return View("Product/New", model);
        }

        
    }
}
