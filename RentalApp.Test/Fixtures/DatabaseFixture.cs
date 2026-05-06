using Xunit;
using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;
using RentalApp.Database.Models;

namespace RentalApp.Test.Fixtures;

// creates a fresh in-memory database for each test class
public class DatabaseFixture : IDisposable
{
    public AppDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        // use a unique name so each test class gets its own database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
        SeedTestData();
    }

    // adds test data so tests have something to work with
    private void SeedTestData()
    {
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Tools" },
            new Category { Id = 2, Name = "Camping" }
        };
        Context.Categories.AddRange(categories);

        var items = new List<Item>
        {
            new Item { Id = 1, Title = "Electric Drill",
                       Description = "Cordless drill",
                       DailyRate = 5.00m, CategoryId = 1,
                       OwnerId = 1, IsAvailable = true }
        };
        Context.Items.AddRange(items);

        var rentals = new List<Rental>
        {
            new Rental { Id = 1, ItemId = 1, BorrowerId = 2,
                         OwnerId = 1, Status = "Requested",
                         StartDate = DateTime.Today.AddDays(1),
                         EndDate   = DateTime.Today.AddDays(3) }
        };
        Context.Rentals.AddRange(rentals);

        Context.SaveChanges();
    }

    // clean up the database after tests are done
    public void Dispose()
    {
        Context.Dispose();
    }
}