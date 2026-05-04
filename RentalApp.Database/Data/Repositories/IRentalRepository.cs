namespace RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

public interface IRentalRepository : IRepository<Rental>
{
    Task<List<Rental>> GetIncomingAsync(string? status = null);
    Task<List<Rental>> GetOutgoingAsync(string? status = null);
    Task UpdateStatusAsync(int rentalId, string status);
}