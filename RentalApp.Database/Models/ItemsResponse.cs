namespace RentalApp.Database.Models;

public class ItemsResponse
{
    public List<Item> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}