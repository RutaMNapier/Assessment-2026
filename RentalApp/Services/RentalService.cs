namespace RentalApp.Services;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

// handles all rental business logic
public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IItemRepository   _itemRepository;

    public RentalService(IRentalRepository rentalRepository, IItemRepository itemRepository)
    {
        _rentalRepository = rentalRepository;
        _itemRepository   = itemRepository;
    }

    // checks if the item is free for the given dates
    // returns false if there is an approved rental that overlaps
    public async Task<bool> CanRentItemAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        var existingRentals = await _rentalRepository.GetIncomingAsync();
        return !existingRentals.Any(r =>
            r.ItemId == itemId &&
            r.Status == "Approved" &&
            r.StartDate < endDate &&
            r.EndDate > startDate);
    }

    // validates the dates and creates a rental request
    public async Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        // start date cannot be in the past
        if (startDate.Date < DateTime.Today)
            throw new ArgumentException("Start date cannot be in the past.");

        // end date must come after start date
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

        // check no approved rental already exists for these dates
        if (!await CanRentItemAsync(itemId, startDate, endDate))
            throw new InvalidOperationException("Item is not available for the selected dates.");

        var rental = new Rental
        {
            ItemId    = itemId,
            StartDate = startDate,
            EndDate   = endDate,
            Status    = "Requested"
        };

        return await _rentalRepository.CreateAsync(rental);
    }

    // get rentals where the current user is the owner
    public Task<List<Rental>> GetIncomingRentalsAsync(string? status = null) =>
        _rentalRepository.GetIncomingAsync(status);

    // get rentals where the current user is the borrower
    public Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null) =>
        _rentalRepository.GetOutgoingAsync(status);

    // owner approves the rental request
    public Task ApproveAsync(int rentalId) =>
        _rentalRepository.UpdateStatusAsync(rentalId, "Approved");

    // owner rejects the rental request
    public Task RejectAsync(int rentalId) =>
        _rentalRepository.UpdateStatusAsync(rentalId, "Rejected");

    // marks the item as returned after the rental period
    public Task ReturnAsync(int rentalId) =>
        _rentalRepository.UpdateStatusAsync(rentalId, "Returned");
}