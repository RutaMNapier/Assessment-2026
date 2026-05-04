using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

public class ApiRentalRepository : IRentalRepository
{
    private readonly IApiService _api;

    public ApiRentalRepository(IApiService api) => _api = api;

    public Task<List<Rental>> GetAllAsync() =>
        _api.GetIncomingRentalsAsync();

    public Task<Rental?> GetByIdAsync(int id) =>
        _api.GetRentalAsync(id)!;

    public async Task<Rental> CreateAsync(Rental entity) =>
        await _api.RequestRentalAsync(entity.ItemId, entity.StartDate, entity.EndDate);

    public Task<Rental> UpdateAsync(Rental entity) =>
        throw new NotSupportedException("Use UpdateStatusAsync instead");

    public Task DeleteAsync(int id) =>
        throw new NotSupportedException("Rental deletion not supported by API");

    public Task<List<Rental>> GetIncomingAsync(string? status = null) =>
        _api.GetIncomingRentalsAsync(status);

    public Task<List<Rental>> GetOutgoingAsync(string? status = null) =>
        _api.GetOutgoingRentalsAsync(status);

    public Task UpdateStatusAsync(int rentalId, string status) =>
        _api.UpdateRentalStatusAsync(rentalId, status);
}