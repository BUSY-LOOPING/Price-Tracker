using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;

[Route("api/tags")]
[ApiController]
public class TagsAPIController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsAPIController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var tags = await _tagService.GetAllAsync();
        return Ok(tags);
    }

    [HttpGet("get")]
    public async Task<ActionResult<TagDto>> GetTag(int id)
    {
        var tag = await _tagService.GetByIdAsync(id);
        if (tag == null)
            return NotFound($"Tag with ID {id} not found.");
        return Ok(tag);
    }

    [HttpPost("add")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto)
    {
        var result = await _tagService.CreateAsync(dto);

        if (result.Status == ServiceResponse<TagDto>.ServiceStatus.Error)
            return BadRequest(result.Messages);

        return Ok(new { message = "Tag created successfully", tagId = result.CreatedId, data = result.Data });
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] CreateTagDto dto)
    {
        var result = await _tagService.UpdateAsync(id, dto);

        return result.Status switch
        {
            ServiceResponse<TagDto>.ServiceStatus.NotFound => NotFound(result.Messages),
            ServiceResponse<TagDto>.ServiceStatus.Error => BadRequest(result.Messages),
            _ => Ok(new { message = "Tag updated", data = result.Data })
        };
    }


    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var result = await _tagService.DeleteAsync(id);

        return result.Status switch
        {
            ServiceResponse<TagDto>.ServiceStatus.NotFound => NotFound(result.Messages),
            ServiceResponse<TagDto>.ServiceStatus.Error => BadRequest(result.Messages),
            _ => Ok(new { message = $"Tag deleted", data = result.Data })
        };
    }

}
