using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
using PriceTracker.Models;

namespace PriceTracker.Controllers
{
    [Route("api/price-entries")]
    [ApiController]
    public class PriceEntryAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PriceEntryAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<ActionResult> CreatePriceEntry([FromBody] PriceEntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var priceEntry = new PriceEntry
            {
                ProductId = dto.ProductId,
                Price = dto.Price,
                Source = dto.Source,
                RecordedAt = DateTime.UtcNow
            };

            _context.PriceEntries.Add(priceEntry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Price entry added", id = priceEntry.EntryId });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePriceEntry(int id, [FromBody] PriceEntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entry = await _context.PriceEntries.FindAsync(id);
            if (entry == null)
                return NotFound($"Entry with ID {id} not found.");

            entry.ProductId = dto.ProductId;
            entry.Price = dto.Price;
            entry.Source = dto.Source;
            entry.RecordedAt = dto.RecordedAt;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Price entry ID {id} updated" });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePriceEntry(int id)
        {
            var entry = await _context.PriceEntries.FindAsync(id);
            if (entry == null)
                return NotFound($"Entry with ID {id} not found.");

            _context.PriceEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Price entry {id} deleted" });
        }

        [HttpGet("get")]
        public async Task<ActionResult<PriceEntryDto>> GetPriceEntry(int id)
        {
            var entry = await _context.PriceEntries.FindAsync(id);
            if (entry == null)
                return NotFound($"Entry with ID {id} not found.");

            var dto = new PriceEntryDto
            {
                EntryId = entry.EntryId,
                ProductId = entry.ProductId,
                Price = entry.Price,
                Source = entry.Source,
                RecordedAt = entry.RecordedAt
            };

            return Ok(dto);
        }

        [HttpGet("product/")]
        public async Task<ActionResult<IEnumerable<PriceEntryDto>>> GetPriceEntriesForProduct([FromQuery] int productId)
        {
            var entries = await _context.PriceEntries
                .Where(pe => pe.ProductId == productId)
                .OrderByDescending(pe => pe.RecordedAt)
                .Select(pe => new PriceEntryDto
                {
                    EntryId = pe.EntryId,
                    ProductId = pe.ProductId,
                    Price = pe.Price,
                    Source = pe.Source,
                    RecordedAt = pe.RecordedAt
                })
                .ToListAsync();

            return Ok(entries);
        }
    }
}
