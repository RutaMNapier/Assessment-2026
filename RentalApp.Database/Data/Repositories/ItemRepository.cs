using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// local database implementation of IItemRepository
// used when useSharedApi = false in MauiProgram.cs
public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context) => _context = context;

    public async Task<List<Item>> GetAllAsync() =>
        await _context.Items.ToListAsync();

    public async Task<Item?> GetByIdAsync(int id) =>
        await _context.Items.FindAsync(id);

    public async Task<Item> CreateAsync(Item entity)
    {
        _context.Items.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Item> UpdateAsync(Item entity)
    {
        var existing = await _context.Items.FindAsync(entity.Id);
        if (existing is null) return entity;
        existing.Title       = entity.Title;
        existing.Description = entity.Description;
        existing.DailyRate   = entity.DailyRate;
        existing.IsAvailable = entity.IsAvailable;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item is null) return;
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Item>> GetByOwnerIdAsync(int ownerId) =>
        await _context.Items.Where(i => i.OwnerId == ownerId).ToListAsync();

    public async Task<List<Item>> SearchAsync(string? category, string? search, int page, int pageSize)
    {
        var query = _context.Items.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(i => i.Title.Contains(search));
        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}