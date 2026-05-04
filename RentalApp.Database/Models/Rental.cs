namespace RentalApp.Database.Models;

public class Rental
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemTitle { get; set; } = "";
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = "";
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Requested";
    public decimal TotalPrice { get; set; }
    public DateTime RequestedAt { get; set; }
}