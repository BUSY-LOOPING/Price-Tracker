using Microsoft.EntityFrameworkCore;
using PriceTracker.Data;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PriceEntryService : IPriceEntryService
{
    private readonly ApplicationDbContext _context;

    public PriceEntryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PriceEntryDto>> GetAllAsync()
    {
        return await _context.PriceEntries
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
    }

    public async Task<ServiceResponse<PriceEntryDto>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<PriceEntryDto>();

        var entry = await _context.PriceEntries.FindAsync(id);
        if (entry == null)
        {
            response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.NotFound;
            response.Messages.Add($"Price entry with ID {id} not found.");
            return response;
        }

        response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.Updated;
        response.Data = new PriceEntryDto
        {
            EntryId = entry.EntryId,
            ProductId = entry.ProductId,
            Price = entry.Price,
            Source = entry.Source,
            RecordedAt = entry.RecordedAt
        };

        return response;
    }

    public async Task<ServiceResponse<PriceEntryDto>> CreateAsync(PriceEntryDto dto)
    {
        var response = new ServiceResponse<PriceEntryDto>();

        if (dto.Price <= 0)
        {
            response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.Error;
            response.Messages.Add("Price must be greater than 0.");
            return response;
        }

        var entry = new PriceEntry
        {
            ProductId = dto.ProductId,
            Price = dto.Price,
            Source = dto.Source,
            RecordedAt = dto.RecordedAt == default ? DateTime.UtcNow : dto.RecordedAt
        };

        _context.PriceEntries.Add(entry);
        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.Created;
        response.CreatedId = entry.EntryId;
        response.Data = new PriceEntryDto
        {
            EntryId = entry.EntryId,
            ProductId = entry.ProductId,
            Price = entry.Price,
            Source = entry.Source,
            RecordedAt = entry.RecordedAt
        };

        return response;
    }

    public async Task<ServiceResponse<PriceEntryDto>> UpdateAsync(int id, PriceEntryDto dto)
    {
        var response = new ServiceResponse<PriceEntryDto>();

        var entry = await _context.PriceEntries.FindAsync(id);
        if (entry == null)
        {
            response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.NotFound;
            response.Messages.Add($"Price entry with ID {id} not found.");
            return response;
        }

        entry.ProductId = dto.ProductId;
        entry.Price = dto.Price;
        entry.Source = dto.Source;
        entry.RecordedAt = dto.RecordedAt;

        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.Updated;
        response.Data = new PriceEntryDto
        {
            EntryId = entry.EntryId,
            ProductId = entry.ProductId,
            Price = entry.Price,
            Source = entry.Source,
            RecordedAt = entry.RecordedAt
        };

        return response;
    }

    public async Task<ServiceResponse<PriceEntryDto>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<PriceEntryDto>();

        var entry = await _context.PriceEntries.FindAsync(id);
        if (entry == null)
        {
            response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.NotFound;
            response.Messages.Add($"Price entry with ID {id} not found.");
            return response;
        }

        _context.PriceEntries.Remove(entry);
        await _context.SaveChangesAsync();

        response.Status = ServiceResponse<PriceEntryDto>.ServiceStatus.Deleted;
        response.Data = new PriceEntryDto
        {
            EntryId = entry.EntryId,
            ProductId = entry.ProductId,
            Price = entry.Price,
            Source = entry.Source,
            RecordedAt = entry.RecordedAt
        };

        return response;
    }

    public async Task<IEnumerable<PriceEntryDto>> GetByProductIdAsync(int productId)
    {
        return await _context.PriceEntries
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
    }
}
