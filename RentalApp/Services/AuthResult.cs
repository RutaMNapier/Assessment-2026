namespace RentalApp.Services;

// holds the login token returned by the API
public class AuthToken
{
    // the token string sent with every request
    public string Token { get; set; } = string.Empty;

    // when the token stops working
    public DateTime ExpiresAt { get; set; }

    // which user this token belongs to
    public int UserId { get; set; }
}