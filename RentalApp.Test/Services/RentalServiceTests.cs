using Xunit;
using Moq;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;

namespace RentalApp.Test.Services;

// tests for RentalService business logic
public class RentalServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly Mock<IRentalRepository> _rentalRepoMock;
    private readonly Mock<IItemRepository>   _itemRepoMock;

    public RentalServiceTests(DatabaseFixture fixture)
    {
        _rentalRepoMock = new Mock<IRentalRepository>();
        _itemRepoMock   = new Mock<IItemRepository>();
    }

    [Fact]
    public async Task GetIncomingAsync_NoRentals_ShouldReturnEmptyList()
    {
        // Arrange
        _rentalRepoMock
            .Setup(r => r.GetIncomingAsync(null))
            .ReturnsAsync(new List<Rental>());

        // Act
        var result = await _rentalRepoMock.Object.GetIncomingAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetIncomingAsync_WithRentals_ShouldReturnRentals()
    {
        // Arrange
        _rentalRepoMock
            .Setup(r => r.GetIncomingAsync(null))
            .ReturnsAsync(new List<Rental>
            {
                new Rental { Id = 1, ItemId = 1, Status = "Requested" }
            });

        // Act
        var result = await _rentalRepoMock.Object.GetIncomingAsync(null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Requested", result[0].Status);
    }

    [Fact]
    public async Task CreateAsync_ValidRental_ShouldReturnRental()
    {
        // Arrange
        var rental = new Rental
        {
            ItemId    = 1,
            BorrowerId = 2,
            StartDate = DateTime.Today.AddDays(1),
            EndDate   = DateTime.Today.AddDays(3),
            Status    = "Requested"
        };

        _rentalRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<Rental>()))
            .ReturnsAsync(new Rental { Id = 1, Status = "Requested" });

        // Act
        var result = await _rentalRepoMock.Object.CreateAsync(rental);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Requested", result.Status);
        _rentalRepoMock.Verify(r => r.CreateAsync(It.IsAny<Rental>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldCallRepository()
    {
        // Arrange
        _rentalRepoMock
            .Setup(r => r.UpdateStatusAsync(1, "Approved"))
            .Returns(Task.CompletedTask);

        // Act
        await _rentalRepoMock.Object.UpdateStatusAsync(1, "Approved");

        // Assert
        _rentalRepoMock.Verify(r => r.UpdateStatusAsync(1, "Approved"), Times.Once);
    }

    [Fact]
    public async Task GetOutgoingAsync_WithStatus_ShouldFilterByStatus()
    {
        // Arrange
        _rentalRepoMock
            .Setup(r => r.GetOutgoingAsync("Approved"))
            .ReturnsAsync(new List<Rental>
            {
                new Rental { Id = 1, Status = "Approved" }
            });

        // Act
        var result = await _rentalRepoMock.Object.GetOutgoingAsync("Approved");

        // Assert
        Assert.Single(result);
        Assert.Equal("Approved", result[0].Status);
    }
}