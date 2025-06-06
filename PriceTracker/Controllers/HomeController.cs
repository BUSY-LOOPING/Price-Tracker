using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductsAPIController _productsApi;
        private readonly TagsAPIController _tagsApi;

        public HomeController(ProductsAPIController productsApi, TagsAPIController tagsApi)
        {
            _productsApi = productsApi;
            _tagsApi = tagsApi;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            var prodResponse = await _productsApi.GetProducts();
            IEnumerable<ProductDto> allProducts = null;

            if (prodResponse.Value != null)
                allProducts = prodResponse.Value;
            else if (prodResponse.Result is OkObjectResult okProd)
                allProducts = okProd.Value as IEnumerable<ProductDto>;

            allProducts ??= new List<ProductDto>();
            vm.TopThreeProducts = allProducts
                .OrderBy(p => p.ProductId)
                .Take(3)
                .ToList();

           //tags
            var tagResponse = await _tagsApi.GetTags();
            IEnumerable<TagDto> allTags = null;
            if (tagResponse.Value != null)
                allTags = tagResponse.Value;
            else if (tagResponse.Result is OkObjectResult okTag)
                allTags = okTag.Value as IEnumerable<TagDto>;
            allTags ??= new List<TagDto>();
            vm.TopThreeTags = allTags
                .OrderBy(t => t.TagId)
                .Take(3)
                .ToList();

            return View(vm);
        }
    }
}
