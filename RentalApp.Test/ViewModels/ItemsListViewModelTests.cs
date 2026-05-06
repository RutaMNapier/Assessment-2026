using Xunit;
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;

namespace RentalApp.Test.ViewModels;

// tests for IItemRepository used by ItemsListViewModel
public class ItemsListViewModelTests : IClassFixture<DatabaseFixture>
{
    private readonly Mock<IItemRepository> _repoMock;

    public ItemsListViewModelTests(DatabaseFixture fixture)
    {
        _repoMock = new Mock<IItemRepository>();
        _repoMock
            .Setup(r => r.SearchAsync(null, null, 1, 20))
            .ReturnsAsync(fixture.Context.Items.ToList());
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnItems()
    {
        // Arrange - mock set up in constructor

        // Act
        var items = await _repoMock.Object.SearchAsync(null, null, 1, 20);

        // Assert
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task SearchAsync_WithKeyword_ShouldReturnMatchingItems()
    {
        // Arrange
        _repoMock
            .Setup(r => r.SearchAsync(null, "drill", 1, 20))
            .ReturnsAsync(new List<Item>
            {
                new Item { Id = 1, Title = "Electric Drill" }
            });

        // Act
        var items = await _repoMock.Object.SearchAsync(null, "drill", 1, 20);

        // Assert
        Assert.Single(items);
        Assert.Equal("Electric Drill", items[0].Title);
    }

    [Fact]
    public async Task SearchAsync_NoResults_ShouldReturnEmptyList()
    {
        // Arrange
        _repoMock
            .Setup(r => r.SearchAsync(null, "xyz", 1, 20))
            .ReturnsAsync(new List<Item>());

        // Act
        var items = await _repoMock.Object.SearchAsync(null, "xyz", 1, 20);

        // Assert
        Assert.Empty(items);
    }
}