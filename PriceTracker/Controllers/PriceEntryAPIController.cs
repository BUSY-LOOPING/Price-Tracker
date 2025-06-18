using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceTracker.Controllers
{
    [Route("api/price-entries")]
    [ApiController]
    public class PriceEntryAPIController : ControllerBase
    {
        private readonly IPriceEntryService _service;

        public PriceEntryAPIController(IPriceEntryService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<PriceEntryDto>>> GetAll()
        {
            var entries = await _service.GetAllAsync();
            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (response.Status == ServiceResponse<PriceEntryDto>.ServiceStatus.NotFound)
                return NotFound(response.Messages);

            return Ok(response.Data);
        }

        [HttpGet("product")]
        public async Task<ActionResult<IEnumerable<PriceEntryDto>>> GetByProduct([FromQuery] int productId)
        {
            var entries = await _service.GetByProductIdAsync(productId);
            return Ok(entries);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PriceEntryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _service.CreateAsync(dto);
            if (response.Status == ServiceResponse<PriceEntryDto>.ServiceStatus.Error)
                return BadRequest(response.Messages);

            return CreatedAtAction(nameof(Get), new { id = response.CreatedId }, response.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PriceEntryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _service.UpdateAsync(id, dto);
            return response.Status switch
            {
                ServiceResponse<PriceEntryDto>.ServiceStatus.NotFound => NotFound(response.Messages),
                ServiceResponse<PriceEntryDto>.ServiceStatus.Error => BadRequest(response.Messages),
                ServiceResponse<PriceEntryDto>.ServiceStatus.Updated => Ok(response.Data),
                _ => BadRequest()
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return response.Status switch
            {
                ServiceResponse<PriceEntryDto>.ServiceStatus.NotFound => NotFound(response.Messages),
                ServiceResponse<PriceEntryDto>.ServiceStatus.Deleted => Ok(response.Data),
                _ => BadRequest(response.Messages)
            };
        }
    }
}
