namespace RentalApp.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string url);
    Task<HttpResponseMessage> PostAsync<T>(string url, T data);
}