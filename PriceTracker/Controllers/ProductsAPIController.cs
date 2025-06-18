using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsAPIController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpPost("add-product")]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.CreateProductAsync(dto);

            if (response.Status != ServiceResponse<ProductDto>.ServiceStatus.Created)
                return BadRequest(response.Messages);

            return Ok(new { message = "Product created successfully", productId = response.Data?.ProductId });
        }

        [HttpPut("update-product")]
        public async Task<ActionResult> UpdateProduct(int productId, [FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.UpdateProductAsync(productId, dto);

            if (response.Status != ServiceResponse<ProductDto>.ServiceStatus.Updated)
                return BadRequest(response.Messages);

            return Ok(new { message = "Product updated successfully", productId = response.Data?.ProductId });
        }

        [HttpDelete("delete-product")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await _productService.DeleteProductAsync(productId);

            if (response.Status == ServiceResponse<bool>.ServiceStatus.NotFound)
                return NotFound($"Product with ID {productId} not found.");

            if (response.Status != ServiceResponse<bool>.ServiceStatus.Deleted)
                return BadRequest(response.Messages);

            return Ok(new { message = $"Product with ID {productId} deleted successfully." });
        }
    }
}
