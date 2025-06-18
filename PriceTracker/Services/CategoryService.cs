using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
using PriceTracker.Interfaces;
using PriceTracker.Models;

namespace PriceTracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            return category == null
                ? null
                : new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                };
        }

        public async Task<ServiceResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto)
        {
            var response = new ServiceResponse<CategoryDto>();

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse<CategoryDto>.ServiceStatus.Created;
            response.CreatedId = category.CategoryId;
            response.Data = new CategoryDto { CategoryId = category.CategoryId, Name = category.Name };

            return response;
        }

        public async Task<ServiceResponse<CategoryDto>> UpdateAsync(int id, CreateCategoryDto dto)
        {
            var response = new ServiceResponse<CategoryDto>();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Status = ServiceResponse<CategoryDto>.ServiceStatus.NotFound;
                response.Messages.Add("Category not found.");
                return response;
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            response.Status = ServiceResponse<CategoryDto>.ServiceStatus.Updated;
            response.Data = new CategoryDto { CategoryId = category.CategoryId, Name = category.Name };

            return response;
        }

        public async Task<ServiceResponse<CategoryDto>> DeleteAsync(int id)
        {
            var response = new ServiceResponse<CategoryDto>();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Status = ServiceResponse<CategoryDto>.ServiceStatus.NotFound;
                response.Messages.Add("Category not found.");
                return response;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse<CategoryDto>.ServiceStatus.Deleted;
            response.Data = new CategoryDto { CategoryId = category.CategoryId, Name = category.Name };

            return response;
        }
    }
}
