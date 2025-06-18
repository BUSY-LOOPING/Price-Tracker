using PriceTracker.Models;

namespace PriceTracker.Interfaces
{
    public interface IPriceEntryService
    {
        Task<IEnumerable<PriceEntryDto>> GetAllAsync();
        Task<ServiceResponse<PriceEntryDto>> GetByIdAsync(int id);
        Task<ServiceResponse<PriceEntryDto>> CreateAsync(PriceEntryDto dto);
        Task<ServiceResponse<PriceEntryDto>> UpdateAsync(int id, PriceEntryDto dto);
        Task<ServiceResponse<PriceEntryDto>> DeleteAsync(int id);
        Task<IEnumerable<PriceEntryDto>> GetByProductIdAsync(int productId);
    }
}
