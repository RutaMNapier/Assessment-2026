namespace RentalApp.Database.Models;

public class UpdateItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; } 
    public bool IsAvailable { get; set; }
}