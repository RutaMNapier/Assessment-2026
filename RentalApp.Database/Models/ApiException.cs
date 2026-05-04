namespace RentalApp.Database.Models;

public class ApiException : Exception
{
    public ApiException(string message) : base(message) { }
}