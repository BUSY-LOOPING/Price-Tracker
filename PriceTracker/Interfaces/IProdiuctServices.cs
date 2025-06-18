using PriceTracker.Models;

namespace PriceTracker.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductDetailsAsync(int productId);
        Task<ServiceResponse<ProductDto>> CreateProductAsync(CreateProductDto dto);
        Task<ServiceResponse<ProductDto>> UpdateProductAsync(int productId, CreateProductDto dto);
        Task<ServiceResponse<bool>> DeleteProductAsync(int productId);
    }
}
