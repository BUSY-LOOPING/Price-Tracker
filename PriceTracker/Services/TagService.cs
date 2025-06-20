using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
using PriceTracker.Interfaces;
using PriceTracker.Models;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _context;

    public TagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        return await _context.Tags
            .Where(t => t.IsActive)
            .Select(t => new TagDto
            {
                TagId = t.TagId,
                Name = t.Name,
                Description = t.Description
            })
            .ToListAsync();
    }

    public async Task<TagDto?> GetByIdAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null || !tag.IsActive) return null;

        return new TagDto
        {
            TagId = tag.TagId,
            Name = tag.Name,
            Description = tag.Description
        };
    }

    public async Task<ServiceResponse<TagDto>> CreateAsync(CreateTagDto dto)
    {
        var response = new ServiceResponse<TagDto>();

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            response.Status = ServiceResponse<TagDto>.ServiceStatus.Error;
            response.Messages.Add("Tag name cannot be empty.");
            return response;
        }

        var tag = new Tag
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<TagDto>.ServiceStatus.Created;
        response.CreatedId = tag.TagId;
        response.Data = new TagDto { TagId = tag.TagId, Name = tag.Name };
        return response;
    }

    public async Task<ServiceResponse<TagDto>> UpdateAsync(int id, CreateTagDto dto)
    {
        var response = new ServiceResponse<TagDto>();

        var tag = await _context.Tags.FindAsync(id);
        if (tag == null || !tag.IsActive)
        {
            response.Status = ServiceResponse<TagDto>.ServiceStatus.NotFound;
            response.Messages.Add($"Tag with ID {id} not found.");
            return response;
        }

        tag.Name = dto.Name;
        tag.Description = dto.Description;

        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<TagDto>.ServiceStatus.Updated;
        response.Data = new TagDto { TagId = tag.TagId, Name = tag.Name };
        return response;
    }

    public async Task<ServiceResponse<TagDto>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<TagDto>();

        var tag = await _context.Tags.FindAsync(id);
        if (tag == null || !tag.IsActive)
        {
            response.Status = ServiceResponse<TagDto>.ServiceStatus.NotFound;
            response.Messages.Add($"Tag with ID {id} not found or already deleted.");
            return response;
        }

        var relatedProductTags = _context.ProductTags.Where(pt => pt.TagId == id);
        _context.ProductTags.RemoveRange(relatedProductTags);

        tag.IsActive = false; // Soft delete
        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<TagDto>.ServiceStatus.Deleted;
        response.Data = new TagDto { TagId = tag.TagId, Name = tag.Name };
        return response;
    }

}
