using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.Test.Fixtures;

namespace RentalApp.Test.Services;

// tests for RentalService business logic
// uses Moq to mock repositories
public class RentalServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly Mock<IRentalRepository> _rentalRepoMock;
    private readonly Mock<IItemRepository> _itemRepoMock;
    private readonly IRentalService _service;

    public RentalServiceTests(DatabaseFixture fixture)
    {
        _rentalRepoMock = new Mock<IRentalRepository>();
        _itemRepoMock   = new Mock<IItemRepository>();
        _service        = new RentalService(_rentalRepoMock.Object, _itemRepoMock.Object);
    }

    [Fact]
    public async Task RequestRentalAsync_StartDateInPast_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(-1);
        var endDate   = DateTime.Today.AddDays(2);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RequestRentalAsync(1, startDate, endDate));
    }

    [Fact]
    public async Task RequestRentalAsync_EndBeforeStart_ShouldThrowArgumentException()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(3);
        var endDate   = DateTime.Today.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RequestRentalAsync(1, startDate, endDate));
    }

    [Fact]
    public async Task RequestRentalAsync_ValidDates_ShouldCreateRental()
    {
        // Arrange
        var startDate = DateTime.Today.AddDays(1);
        var endDate   = DateTime.Today.AddDays(3);

        _rentalRepoMock
            .Setup(r => r.GetIncomingAsync(null))
            .ReturnsAsync(new List<Rental>());

        _rentalRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Rental>()))
            .ReturnsAsync(new Rental
            {
                Id = 1, ItemId = 1,
                StartDate = startDate, EndDate = endDate,
                Status = "Requested"
            });

        // Act
        var result = await _service.RequestRentalAsync(1, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Requested", result.Status);
        _rentalRepoMock.Verify(r => r.CreateAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task CanRentItemAsync_NoConflicts_ShouldReturnTrue()
    {
        // Arrange
        _rentalRepoMock
            .Setup(r => r.GetIncomingAsync(null))
            .ReturnsAsync(new List<Rental>());

        // Act
        var result = await _service.CanRentItemAsync(1,
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(3));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanRentItemAsync_WithConflict_ShouldReturnFalse()
    {
        // Arrange
        _rentalRepoMock
            .Setup(r => r.GetIncomingAsync(null))
            .ReturnsAsync(new List<Rental>
            {
                new Rental
                {
                    ItemId    = 1,
                    Status    = "Approved",
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate   = DateTime.Today.AddDays(5)
                }
            });

        // Act
        var result = await _service.CanRentItemAsync(1,
            DateTime.Today.AddDays(2),
            DateTime.Today.AddDays(4));

        // Assert
        Assert.False(result);
    }
}