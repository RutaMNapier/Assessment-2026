namespace RentalApp.Database.Models;

public class ReviewsResponse
{
    public List<Review> Reviews { get; set; } = [];
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}