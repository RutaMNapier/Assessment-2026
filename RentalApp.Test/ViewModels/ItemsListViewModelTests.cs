using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;

namespace RentalApp.Test.ViewModels;

// tests for ItemsListViewModel
// uses Moq to mock IItemRepository
public class ItemsListViewModelTests : IClassFixture<DatabaseFixture>
{
    private readonly Mock<IItemRepository> _repoMock;
    private readonly ItemsListViewModel _vm;

    public ItemsListViewModelTests(DatabaseFixture fixture)
    {
        _repoMock = new Mock<IItemRepository>();
        _vm = new ItemsListViewModel(_repoMock.Object);
    }

    [Fact]
    public async Task LoadItemsCommand_ShouldPopulateItems()
    {
        // Arrange
        _repoMock
            .Setup(r => r.SearchAsync(null, null, 1, 20))
            .ReturnsAsync(new List<Item>
            {
                new Item { Id = 1, Title = "Electric Drill" },
                new Item { Id = 2, Title = "Camping Tent" }
            });

        // Act
        await _vm.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, _vm.Items.Count);
        Assert.False(_vm.IsBusy);
    }

    [Fact]
    public async Task LoadItemsCommand_WhenApiFails_ShouldSetErrorMessage()
    {
        // Arrange
        _repoMock
            .Setup(r => r.SearchAsync(null, null, 1, 20))
            .ThrowsAsync(new ApiException("Network error"));

        // Act
        await _vm.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.NotEmpty(_vm.ErrorMessage!);
        Assert.False(_vm.IsBusy);
    }

    [Fact]
    public async Task LoadItemsCommand_EmptyResult_ShouldHaveEmptyCollection()
    {
        // Arrange
        _repoMock
            .Setup(r => r.SearchAsync(null, null, 1, 20))
            .ReturnsAsync(new List<Item>());

        // Act
        await _vm.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.Empty(_vm.Items);
        Assert.False(_vm.IsBusy);
    }
}