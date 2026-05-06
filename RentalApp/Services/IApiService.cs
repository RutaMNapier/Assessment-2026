using RentalApp.Database.Models;

namespace RentalApp.Services;

// defines all the actions the app can do with the API
public interface IApiService
{
    // Auth - login, register, get current user
    Task<AuthToken> LoginAsync(string email, string password);
    Task<User> RegisterAsync(string firstName, string lastName, string email, string password);
    Task<User> GetCurrentUserAsync();

    // Items, browse, view, create, update listings
    Task<List<Item>> GetItemsAsync(string? category = null, string? search = null, int page = 1);
    Task<Item> GetItemAsync(int id);
    Task<Item> CreateItemAsync(CreateItemRequest request);
    Task<Item> UpdateItemAsync(int id, UpdateItemRequest request);
    Task<List<Category>> GetCategoriesAsync();

    // Rentals - request, view, update rental status
    Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate);
    Task<List<Rental>> GetIncomingRentalsAsync(string? status = null);
    Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null);
    Task<Rental> GetRentalAsync(int id);
    Task UpdateRentalStatusAsync(int rentalId, string status);

    // Reviews
    Task<Review> CreateReviewAsync(int rentalId, int rating, string comment);
    Task<List<Review>> GetItemReviewsAsync(int itemId);
}