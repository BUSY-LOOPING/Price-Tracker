using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
//using PriceTracker.Migrations;
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

        [HttpPost("add-product")]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //creating or fetching company
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == dto.CompanyName);
            if (company == null)
            {
                company = new Company
                {
                    Name = dto.CompanyName                    
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }

            var product = new Product
            {
                Name = dto.Name,
                Url = dto.Url,
                Description = dto.Description,
                CompanyId = company.CompanyId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //Adding a price entry
            var priceEntry = new PriceEntry
            {
                ProductId = product.ProductId,
                Price = dto.CurrentPrice,
                Source = dto.PriceSource,
                RecordedAt = DateTime.UtcNow
            };
            _context.PriceEntries.Add(priceEntry);
            await _context.SaveChangesAsync();

            //handling tags
            foreach(var tagName in dto.Tags.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName
                    };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }
                _context.ProductTags.Add(new ProductTag
                {
                    ProductId = product.ProductId,
                    TagId = tag.TagId,
                    TagSource = dto.TagSource
                });
            }
            await _context.SaveChangesAsync();

            //handle categories
            foreach (var catName in dto.CategoryNames.Distinct())
            {
                var cat = await _context.Categories.FirstOrDefaultAsync(c => c.Name == catName);
                if (cat == null)
                {
                    cat = new Category
                    {
                        Name = catName
                    };
                    _context.Categories.Add(cat);
                    await _context.SaveChangesAsync();
                }
                _context.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.ProductId,
                    CategoryId = cat.CategoryId
                });
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product created successfully", productId = product.ProductId });
        }

        [HttpPut("update-product")]
        public async Task<ActionResult> UpdateProduct(int productId, [FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products
                                .Include(p => p.PriceEntries)
                                .Include(p => p.ProductTags)
                                .Include(p => p.ProductCategories)
                                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return NotFound($"Product with ID {productId} not found.");

            // Update core fields
            product.Name = dto.Name;
            product.Url = dto.Url;
            product.Description = dto.Description;


            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == dto.CompanyName);
            if (company == null)
            {
                company = new Company { Name = dto.CompanyName };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }
            product.CompanyId = company.CompanyId;

            if (product.ProductTags != null)
                _context.ProductTags.RemoveRange(product.ProductTags);

            foreach(var tagName in dto.Tags.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName};
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                _context.ProductTags.Add(new ProductTag
                {
                    ProductId = product.ProductId,
                    TagId = tag.TagId
                });
            }
            await _context.SaveChangesAsync();

            // Remove old categories
            if (product.ProductCategories != null)
                _context.ProductCategories.RemoveRange(product.ProductCategories);
            await _context.SaveChangesAsync();


            foreach (var catName in dto.CategoryNames.Distinct())
            {
                var cat = await _context.Categories.FirstOrDefaultAsync(c => c.Name == catName);
                if (cat == null)
                {
                    cat = new Category { Name = catName };
                    _context.Categories.Add(cat);
                    await _context.SaveChangesAsync();
                }

                _context.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.ProductId,
                    CategoryId = cat.CategoryId
                });
            }
            await _context.SaveChangesAsync();


            return Ok(new { message = "Product updated successfully", productId = product.ProductId });
        }

        [HttpDelete("delete-product")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound($"Product with ID {productId} not found.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Product with ID {productId} deleted successfully." });
        }

        [HttpPost("link-product-to-category")]
        public async Task<ActionResult> LinkProductToCategory(int productId, [FromBody] string categoryName)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound($"Product with ID {productId} not found.");

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                category = new Category { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            bool alreadyLinked = await _context.ProductCategories
                .AnyAsync(pc => pc.ProductId == productId && pc.CategoryId == category.CategoryId);

            if (!alreadyLinked)
            {
                _context.ProductCategories.Add(new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = category.CategoryId
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Product {productId} linked to category '{categoryName}'." });
        }

        [HttpDelete("unlink-product-from-category")]
        public async Task<ActionResult> UnlinkProductFromCategory(int productId, [FromBody] string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
                return NotFound($"Category '{categoryName}' not found.");

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == category.CategoryId);

            if (productCategory == null)
                return NotFound($"Link between product {productId} and category '{categoryName}' not found.");

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Product {productId} unlinked from category '{categoryName}'." });
        }

    }
}
