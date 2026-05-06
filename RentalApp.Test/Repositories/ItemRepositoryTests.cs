using Xunit;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;

namespace RentalApp.Test.Repositories;

// tests for the ItemRepository class using the in memory database
public class ItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ItemRepository  _repository;

    // gets the database fixture and creates a real repository to test
    public ItemRepositoryTests(DatabaseFixture fixture)
    {
        _fixture    = fixture;
        _repository = new ItemRepository(_fixture.Context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        // Arrange - data seeded in fixture

        // Act
        var items = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnItem()
    {
        // Arrange
        var expectedId = 1;

        // Act
        var item = await _repository.GetByIdAsync(expectedId);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(expectedId, item!.Id);
        Assert.Equal("Electric Drill", item.Title);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var item = await _repository.GetByIdAsync(invalidId);

        // Assert
        Assert.Null(item);
    }

    [Fact]
    public async Task CreateAsync_ValidItem_ShouldAddToDatabase()
    {
        // Arrange
        var newItem = new Item
        {
            Title       = "Camping Tent",
            Description = "4-person tent",
            DailyRate   = 15.00m,
            CategoryId  = 2,
            OwnerId     = 1
        };

        // Act
        var created = await _repository.CreateAsync(newItem);

        // Assert
        Assert.NotNull(created);
        Assert.NotEqual(0, created.Id);
        Assert.Equal("Camping Tent", created.Title);
    }
}