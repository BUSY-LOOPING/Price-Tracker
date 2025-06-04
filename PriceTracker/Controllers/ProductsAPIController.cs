using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
using PriceTracker.Models;

namespace PriceTracker.Controllers
{
    [Route("api/products")]
    [ApiController]

    public class ProductsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Company)
                .Include(p => p.PriceEntries)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Url = p.Url,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    CompanyId = p.CompanyId,
                    CompanyName = p.Company.Name,

                    // Getting the latest price entry details
                    LatestPrice = p.PriceEntries
                                    .OrderByDescending(pe => pe.RecordedAt)
                                    .Select(pe => (decimal?)pe.Price)
                                    .FirstOrDefault(),

                    LatestPriceRecordedAt = p.PriceEntries
                                    .OrderByDescending(pe => pe.RecordedAt)
                                    .Select(pe => (DateTime?)pe.RecordedAt)
                                    .FirstOrDefault(),

                    LatestPriceSource = p.PriceEntries
                                    .OrderByDescending(pe => pe.RecordedAt)
                                    .Select(pe => pe.Source)
                                    .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}
