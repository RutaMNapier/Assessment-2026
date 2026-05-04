namespace RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

public interface IItemRepository : IRepository<Item>
{
    Task<List<Item>> GetByOwnerIdAsync(int ownerId);
    Task<List<Item>> SearchAsync(string? category, string? search, int page, int pageSize);
}
