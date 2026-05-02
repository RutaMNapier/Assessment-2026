using System.Net.Http.Json;

namespace RentalApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        return await _httpClient.GetFromJsonAsync<T>(url);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {
        return await _httpClient.PostAsJsonAsync(url, data);
    }
}