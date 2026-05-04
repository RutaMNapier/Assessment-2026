namespace RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

public interface IReviewRepository : IRepository<Review>
{
    Task<List<Review>> GetByItemIdAsync(int itemId);
}