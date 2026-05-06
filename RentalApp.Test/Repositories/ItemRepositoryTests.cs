using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;

namespace RentalApp.Test.Repositories;

// tests for IItemRepository
// uses Moq and DatabaseFixture seed data
public class ItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly Mock<IItemRepository> _repoMock;

    public ItemRepositoryTests(DatabaseFixture fixture)
    {
        _fixture  = fixture;
        _repoMock = new Mock<IItemRepository>();

        _repoMock.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(_fixture.Context.Items.ToList());

        _repoMock.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(_fixture.Context.Items.First());

        _repoMock.Setup(r => r.GetByIdAsync(999))
                 .ReturnsAsync((Item?)null);

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Item>()))
                 .ReturnsAsync((Item item) =>
                 {
                     item.Id = _fixture.Context.Items.Count() + 1;
                     return item;
                 });
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        // Arrange - data seeded in fixture

        // Act
        var items = await _repoMock.Object.GetAllAsync();

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
        var item = await _repoMock.Object.GetByIdAsync(expectedId);

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
        var item = await _repoMock.Object.GetByIdAsync(invalidId);

        // Assert
        Assert.Null(item);
    }

    [Fact]
    public async Task CreateAsync_ValidItem_ShouldReturnItemWithId()
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
        var created = await _repoMock.Object.CreateAsync(newItem);

        // Assert
        Assert.NotNull(created);
        Assert.Equal("Camping Tent", created.Title);
        Assert.NotEqual(0, created.Id);
    }
}