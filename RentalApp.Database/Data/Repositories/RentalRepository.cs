using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;
using RentalApp.Database.Models;

namespace RentalApp.Database.Data.Repositories;

// local database implementation of IRentalRepository
// used when useSharedApi = false in MauiProgram.cs
public class RentalRepository : IRentalRepository
{
    private readonly AppDbContext _context;

    public RentalRepository(AppDbContext context) => _context = context;

    public async Task<List<Rental>> GetAllAsync() =>
        await _context.Rentals.ToListAsync();

    public async Task<Rental?> GetByIdAsync(int id) =>
        await _context.Rentals.FindAsync(id);

    public async Task<Rental> CreateAsync(Rental entity)
    {
        _context.Rentals.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Rental> UpdateAsync(Rental entity)
    {
        _context.Rentals.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental is null) return;
        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Rental>> GetIncomingAsync(string? status = null) =>
        await _context.Rentals.Where(r => status == null || r.Status == status).ToListAsync();

    public async Task<List<Rental>> GetOutgoingAsync(string? status = null) =>
        await _context.Rentals.Where(r => status == null || r.Status == status).ToListAsync();

    public async Task UpdateStatusAsync(int rentalId, string status)
    {
        var rental = await _context.Rentals.FindAsync(rentalId);
        if (rental is null) return;
        rental.Status = status;
        await _context.SaveChangesAsync();
    }
}