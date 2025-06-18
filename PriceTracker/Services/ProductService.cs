using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
using PriceTracker.Interfaces;
using PriceTracker.Models;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Company)
            .Include(p => p.PriceEntries)
            .Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Url = p.Url,
                Description = p.Description,
                Condition = p.Condition,
                CreatedAt = p.CreatedAt,
                CompanyId = p.CompanyId,
                CompanyName = p.Company.Name,
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
    }

    public async Task<ProductDto?> GetProductDetailsAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Company)
            .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.PriceEntries)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null) return null;

        var priceEntries = product.PriceEntries.OrderByDescending(pe => pe.RecordedAt).ToList();

        decimal? maxPrice = priceEntries.Any() ? priceEntries.Max(pe => pe.Price) : null;
        decimal? minPrice = priceEntries.Any() ? priceEntries.Min(pe => pe.Price) : null;
        decimal? avgPrice = priceEntries.Any() ? priceEntries.Average(pe => pe.Price) : null;

        decimal? priceChange = null;
        double? priceChangePercent = null;
        if (priceEntries.Count > 1)
        {
            var latest = priceEntries[0].Price;
            var previous = priceEntries[1].Price;
            priceChange = latest - previous;
            if (previous != 0)
                priceChangePercent = (double)(priceChange / previous) * 100;
        }

        return new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Url = product.Url,
            Condition = product.Condition,
            Description = product.Description,
            CreatedAt = product.CreatedAt,
            CompanyName = product.Company.Name,
            Categories = product.ProductCategories.Select(pc => pc.Category.Name).ToList(),
            Tags = product.ProductTags.Select(pt => pt.Tag.Name).ToList(),
            LatestPrice = priceEntries.FirstOrDefault()?.Price,
            LatestPriceSource = priceEntries.FirstOrDefault()?.Source,
            LatestPriceRecordedAt = priceEntries.FirstOrDefault()?.RecordedAt,
            MaxPrice = maxPrice,
            MinPrice = minPrice,
            AveragePrice = avgPrice,
            PriceChangeSinceLast = priceChange,
            PriceChangePercentage = priceChangePercent
        };
    }

    public async Task<ServiceResponse<ProductDto>> CreateProductAsync(CreateProductDto dto)
    {
        var response = new ServiceResponse<ProductDto>();

        var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
        if (company == null)
        {
            response.Status = ServiceResponse<ProductDto>.ServiceStatus.Error;
            response.Messages.Add($"Company with ID {dto.CompanyId} not found.");
            return response;
        }

        var product = new Product
        {
            Name = dto.Name,
            Url = dto.Url,
            Description = dto.Description,
            Condition = dto.Condition?.ToString(),
            CompanyId = dto.CompanyId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var priceEntry = new PriceEntry
        {
            ProductId = product.ProductId,
            Price = dto.CurrentPrice,
            Source = dto.PriceSource,
            RecordedAt = DateTime.UtcNow
        };
        _context.PriceEntries.Add(priceEntry);

        // Tags
        foreach (var tagName in dto.Tags.Distinct())
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
            _context.ProductTags.Add(new ProductTag
            {
                ProductId = product.ProductId,
                TagId = tag.TagId
            });
        }

        // Categories
        foreach (var categoryName in dto.CategoryNames.Distinct())
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                category = new Category { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            _context.ProductCategories.Add(new ProductCategory
            {
                ProductId = product.ProductId,
                CategoryId = category.CategoryId
            });
        }

        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<ProductDto>.ServiceStatus.Created;
        response.CreatedId = product.ProductId;
        response.Data = new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Url = product.Url,
            Description = product.Description,
            Condition = dto.Condition,
            CreatedAt = product.CreatedAt,
            CompanyId = product.CompanyId,
            CompanyName = company.Name,
            LatestPrice = priceEntry.Price,
            LatestPriceRecordedAt = priceEntry.RecordedAt,
            LatestPriceSource = priceEntry.Source
        };

        return response;
    }

    public async Task<ServiceResponse<ProductDto>> UpdateProductAsync(int productId, CreateProductDto dto)
    {
        var response = new ServiceResponse<ProductDto>();

        var product = await _context.Products
            .Include(p => p.ProductTags)
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
        {
            response.Status = ServiceResponse<ProductDto>.ServiceStatus.NotFound;
            response.Messages.Add($"Product with ID {productId} not found.");
            return response;
        }

        var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == dto.CompanyId);
        if (company == null)
        {
            response.Status = ServiceResponse<ProductDto>.ServiceStatus.Error;
            response.Messages.Add($"Company with ID {dto.CompanyId} not found.");
            return response;
        }

        product.Name = dto.Name;
        product.Url = dto.Url;
        product.Description = dto.Description;
        product.Condition = dto.Condition?.ToString();
        product.CompanyId = dto.CompanyId;

        if (product.ProductTags != null)
            _context.ProductTags.RemoveRange(product.ProductTags);

        if (dto.Tags != null)
        {
            foreach (var tagName in dto.Tags.Distinct())
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }
                _context.ProductTags.Add(new ProductTag
                {
                    ProductId = product.ProductId,
                    TagId = tag.TagId
                });
            }
        }

        if (product.ProductCategories != null)
            _context.ProductCategories.RemoveRange(product.ProductCategories);

        foreach (var categoryName in dto.CategoryNames.Distinct())
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                category = new Category { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }
            _context.ProductCategories.Add(new ProductCategory
            {
                ProductId = product.ProductId,
                CategoryId = category.CategoryId
            });
        }

        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<ProductDto>.ServiceStatus.Updated;
        response.Data = new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Url = product.Url,
            Description = product.Description,
            Condition = dto.Condition,
            CreatedAt = product.CreatedAt,
            CompanyId = product.CompanyId,
            CompanyName = company.Name
        };

        return response;
    }

    public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
    {
        var response = new ServiceResponse<bool>();

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            response.Status = ServiceResponse<bool>.ServiceStatus.NotFound;
            response.Messages.Add($"Product with ID {productId} not found.");
            response.Data = false;
            return response;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<bool>.ServiceStatus.Deleted;
        response.Data = true;

        return response;
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Company)
            .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .Include(p => p.PriceEntries)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null) return null;

        var latest = product.PriceEntries?
            .OrderByDescending(pe => pe.RecordedAt)
            .FirstOrDefault();

        var previous = product.PriceEntries?
            .OrderByDescending(pe => pe.RecordedAt)
            .Skip(1)
            .FirstOrDefault();

        decimal? priceChange = latest != null && previous != null
            ? latest.Price - previous.Price
            : null;

        double? priceChangePercent = (latest != null && previous != null && previous.Price != 0)
            ? (double)(priceChange.Value / previous.Price) * 100
            : null;

        return new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Url = product.Url,
            Condition = product.Condition,
            Description = product.Description,
            CreatedAt = product.CreatedAt,
            CompanyName = product.Company?.Name ?? "",

            Categories = product.ProductCategories?.Select(pc => pc.Category.Name).ToList() ?? new(),
            Tags = product.ProductTags?.Select(pt => pt.Tag.Name).ToList() ?? new(),

            LatestPrice = latest?.Price,
            LatestPriceSource = latest?.Source,
            LatestPriceRecordedAt = latest?.RecordedAt,

            MaxPrice = product.PriceEntries?.Max(pe => pe.Price),
            MinPrice = product.PriceEntries?.Min(pe => pe.Price),
            AveragePrice = product.PriceEntries?.Average(pe => pe.Price),
            PriceChangeSinceLast = priceChange,
            PriceChangePercentage = priceChangePercent
        };
    }

}
