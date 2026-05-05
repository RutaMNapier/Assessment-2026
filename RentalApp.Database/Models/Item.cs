namespace RentalApp.Database.Models;

public class Item
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public double? OwnerRating { get; set; }      
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsAvailable { get; set; }
    public double? AverageRating { get; set; }    
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}