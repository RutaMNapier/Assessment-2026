namespace RentalApp.Database.Models;

public class RentalsResponse
{
    public List<Rental> Rentals { get; set; } = [];
    public int TotalRentals { get; set; }
}