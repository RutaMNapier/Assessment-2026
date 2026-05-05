namespace RentalApp.Services;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IItemRepository _itemRepository;

    public RentalService(IRentalRepository rentalRepository, IItemRepository itemRepository)
    {
        _rentalRepository = rentalRepository;
        _itemRepository   = itemRepository;
    }

    public async Task<bool> CanRentItemAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        var existingRentals = await _rentalRepository.GetIncomingAsync();
        return !existingRentals.Any(r =>
            r.ItemId == itemId &&
            r.Status == "Approved" &&
            r.StartDate < endDate &&
            r.EndDate > startDate);
    }

    public async Task<Rental> RequestRentalAsync(int itemId, DateTime startDate, DateTime endDate)
    {
        if (startDate.Date < DateTime.Today)
            throw new ArgumentException("Start date cannot be in the past.");

        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

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

    public Task<List<Rental>> GetIncomingRentalsAsync(string? status = null) =>
        _rentalRepository.GetIncomingAsync(status);

    public Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null) =>
        _rentalRepository.GetOutgoingAsync(status);

    public Task ApproveAsync(int rentalId) =>
        _rentalRepository.UpdateStatusAsync(rentalId, "Approved");

    public Task RejectAsync(int rentalId) =>
        _rentalRepository.UpdateStatusAsync(rentalId, "Rejected");

    public Task ReturnAsync(int rentalId) =>
        _rentalRepository.UpdateStatusAsync(rentalId, "Returned");
}