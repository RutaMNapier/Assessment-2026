namespace RentalApp.Services;
using RentalApp.Database.Models;

public interface IRentalService
{
    Task<bool> CanRentItemAsync(int itemId, DateTime startDate, DateTime endDate);
    Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate);
    Task<List<Rental>> GetIncomingRentalsAsync(string? status = null);  // ← add
    Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null);  // ← add
    Task ApproveAsync(int rentalId);
    Task RejectAsync(int rentalId);
    Task ReturnAsync(int rentalId);
}