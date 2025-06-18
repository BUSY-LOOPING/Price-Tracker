using PriceTracker.Models;

namespace PriceTracker.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto?> GetByIdAsync(int id);
        Task<ServiceResponse<TagDto>> CreateAsync(CreateTagDto dto);
        Task<ServiceResponse<TagDto>> UpdateAsync(int id, CreateTagDto dto);
        Task<ServiceResponse<TagDto>> DeleteAsync(int id);
    }

}
