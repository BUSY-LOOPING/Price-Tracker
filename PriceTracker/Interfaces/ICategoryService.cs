using PriceTracker.Models;

namespace PriceTracker.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<ServiceResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto);
        Task<ServiceResponse<CategoryDto>> UpdateAsync(int id, CreateCategoryDto dto);
        Task<ServiceResponse<CategoryDto>> DeleteAsync(int id);
    }
}
