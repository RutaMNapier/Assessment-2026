namespace RentalApp.Repositories;

using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

// Api implementation of IItemRepository
// all data access delegated to IApiService
public class ApiItemRepository : IItemRepository
{
    private readonly IApiService _api;

    public ApiItemRepository(IApiService api) => _api = api;

    // get all items from API
    public Task<List<Item>> GetAllAsync() =>
        _api.GetItemsAsync();

    // get single item by id
    public Task<Item?> GetByIdAsync(int id) =>
        _api.GetItemAsync(id)!;

    // create new item listing via POST /items
    public Task<Item> CreateAsync(Item entity) =>
        _api.CreateItemAsync(new CreateItemRequest
        {
            Title       = entity.Title,
            Description = entity.Description,
            DailyRate   = entity.DailyRate,
            CategoryId  = entity.CategoryId,
            Latitude    = entity.Latitude ?? 0,   // default to 0 if null
            Longitude   = entity.Longitude ?? 0   // default to 0 if null
        });

    // update existing item via PUT /items/{id}
    // API enforces owner only access
    public Task<Item> UpdateAsync(Item entity) =>
        _api.UpdateItemAsync(entity.Id, new UpdateItemRequest
        {
            Title       = entity.Title,
            Description = entity.Description,
            DailyRate   = entity.DailyRate,
            CategoryId  = entity.CategoryId,
            IsAvailable = entity.IsAvailable
        });

    // deletion not supported by the shared API
    public Task DeleteAsync(int id) =>
        throw new NotSupportedException("Item deletion not supported by API");

    // get items by owner, filtering done client side
    public Task<List<Item>> GetByOwnerIdAsync(int ownerId) =>
        _api.GetItemsAsync();

    // search items by category and keyword via GET /items
    public Task<List<Item>> SearchAsync(string? category, string? search, int page, int pageSize) =>
        _api.GetItemsAsync(category, search, page);
}