using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Data;
using PriceTracker.Models;

namespace PriceTracker.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _context.Tags
                .Where(t => t.IsActive)
                .Select(t => new TagDto
                {
                    TagId = t.TagId,
                    Name = t.Name
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpGet("get")]
        public async Task<ActionResult<TagDto>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null || !tag.IsActive)
                return NotFound($"Tag with ID {id} not found.");

            var dto = new TagDto
            {
                TagId = tag.TagId,
                Name = tag.Name
            };

            return Ok(dto);
        }

        [HttpPost("add")]
        public async Task<ActionResult> CreateTag([FromBody] TagDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tag = new Tag
            {
                Name = dto.Name,
                IsActive = true
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tag created successfully", tagId = tag.TagId });
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateTag(int id, [FromBody] TagDto dto)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null || !tag.IsActive)
                return NotFound($"Tag with ID {id} not found.");

            tag.Name = dto.Name;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Tag with ID {id} updated" });
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null || !tag.IsActive)
                return NotFound($"Tag with ID {id} not found.");

            tag.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Tag with ID {id} deleted" });
        }

    }
}
