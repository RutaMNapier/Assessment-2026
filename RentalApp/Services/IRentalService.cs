namespace RentalApp.Services;
using RentalApp.Database.Models;
public interface IRentalService
{
    Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate);
    Task ApproveAsync(int rentalId);
    Task RejectAsync(int rentalId);
}
