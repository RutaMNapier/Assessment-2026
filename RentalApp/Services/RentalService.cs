namespace RentalApp.Services;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentals;  // ← declare the field

    public RentalService(IRentalRepository rentals) => _rentals = rentals;

    public async Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        if (startDate.Date < DateTime.Today)
            throw new ArgumentException("Start date cannot be in the past.");

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

        var rental = new Rental
        {
            ItemId    = itemId,
            StartDate = startDate,
            EndDate   = endDate,
            Status    = "Requested"
        };

        var result = await _rentals.CreateAsync(rental);  // ← _rentals not _rentalRepository
        return result ?? throw new InvalidOperationException("Rental request failed.");
    }

    public Task ApproveAsync(int rentalId) =>
        _rentals.UpdateStatusAsync(rentalId, "Approved");

    public Task RejectAsync(int rentalId) =>
        _rentals.UpdateStatusAsync(rentalId, "Rejected");
}