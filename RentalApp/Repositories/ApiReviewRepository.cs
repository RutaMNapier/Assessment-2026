using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

public class ApiReviewRepository : IReviewRepository
{
    private readonly IApiService _api;

    public ApiReviewRepository(IApiService api) => _api = api;

    public Task<List<Review>> GetAllAsync() =>
        throw new NotSupportedException("Use GetByItemIdAsync instead");

    public Task<Review?> GetByIdAsync(int id) =>
        throw new NotSupportedException("Not supported by API");

    public async Task<Review> CreateAsync(Review entity) =>
        await _api.CreateReviewAsync(entity.RentalId, entity.Rating, entity.Comment);

    public Task<Review> UpdateAsync(Review entity) =>
        throw new NotSupportedException("Reviews cannot be updated");

    public Task DeleteAsync(int id) =>
        throw new NotSupportedException("Reviews cannot be deleted");

    public Task<List<Review>> GetByItemIdAsync(int itemId) =>
        _api.GetItemReviewsAsync(itemId);
}