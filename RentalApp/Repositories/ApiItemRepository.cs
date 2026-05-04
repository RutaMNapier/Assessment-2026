using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

public class ApiItemRepository : IItemRepository
{
    private readonly IApiService _api;

    public ApiItemRepository(IApiService api) => _api = api;

    public Task<List<Item>> GetAllAsync() =>
        _api.GetItemsAsync();

    public Task<Item?> GetByIdAsync(int id) =>
        _api.GetItemAsync(id)!;

    public Task<Item> CreateAsync(Item entity) =>
        _api.CreateItemAsync(new CreateItemRequest
        {
            Title       = entity.Title,
            Description = entity.Description,
            DailyRate   = entity.DailyRate,
            CategoryId  = entity.CategoryId,
            Latitude    = entity.Latitude,
            Longitude   = entity.Longitude
        });

    public Task<Item> UpdateAsync(Item entity) =>
        _api.UpdateItemAsync(entity.Id, new UpdateItemRequest
        {
            Title       = entity.Title,
            Description = entity.Description,
            DailyRate   = entity.DailyRate,
            IsAvailable = entity.IsAvailable
        });

    public Task DeleteAsync(int id) =>
        throw new NotSupportedException("Item deletion not supported by API");

    public Task<List<Item>> GetByOwnerIdAsync(int ownerId) =>
        _api.GetItemsAsync();   // filter client-side on OwnerId

    public Task<List<Item>> SearchAsync(string? category, string? search, int page, int pageSize) =>
        _api.GetItemsAsync(category, search, page);
}