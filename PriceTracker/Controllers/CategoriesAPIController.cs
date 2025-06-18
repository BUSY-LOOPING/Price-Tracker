using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using PriceTracker.Services;

namespace PriceTracker.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound($"Category with ID {id} not found.");
            return Ok(category);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _categoryService.CreateAsync(dto);

            return result.Status switch
            {
                ServiceResponse<CategoryDto>.ServiceStatus.Created => Ok(result),
                _ => BadRequest(result.Messages)
            };
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);

            return result.Status switch
            {
                ServiceResponse<CategoryDto>.ServiceStatus.Updated => Ok(result),
                ServiceResponse<CategoryDto>.ServiceStatus.NotFound => NotFound(result.Messages),
                _ => BadRequest(result.Messages)
            };
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);

            return result.Status switch
            {
                ServiceResponse<CategoryDto>.ServiceStatus.Deleted => Ok(result),
                ServiceResponse<CategoryDto>.ServiceStatus.NotFound => NotFound(result.Messages),
                _ => BadRequest(result.Messages)
            };
        }
    }
}
