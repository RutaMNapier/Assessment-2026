namespace RentalApp.Services;
using RentalApp.Database.Models;
using System.Net.Http.Json;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<T?> GetAsync<T>(string url)
    {return await _httpClient.GetFromJsonAsync<T>(url);}

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
    {return await _httpClient.PostAsJsonAsync(url, data);}

    private async Task<HttpRequestMessage> AuthRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        var token = await SecureStorage.Default.GetAsync("jwt_token");
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(err?.Message ?? response.ReasonPhrase ?? "Request failed");
        }
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new ApiException("Empty response from API");
    }

    // Auth
    public async Task<AuthToken> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/token",
            new { email, password });
        if (!response.IsSuccessStatusCode)
            throw new ApiException("Invalid email or password");
        return await response.Content.ReadFromJsonAsync<AuthToken>()
            ?? throw new ApiException("Empty response");
    }

    public async Task<User> RegisterAsync(string firstName, string lastName,
        string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/register",
            new { firstName, lastName, email, password });
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(err?.Message ?? "Registration failed");
        }
        return await response.Content.ReadFromJsonAsync<User>()
            ?? throw new ApiException("Empty response");
    }

    public async Task<User> GetCurrentUserAsync()
    {
        var req = await AuthRequest(HttpMethod.Get, "/users/me");
        return await SendAsync<User>(req);
    }

    // tems
    public async Task<List<Item>> GetItemsAsync(string? category = null,
        string? search = null, int page = 1)
    {
        var url = $"/items?page={page}&pageSize=20";
        if (!string.IsNullOrEmpty(category))
            url += $"&category={Uri.EscapeDataString(category)}";
        if (!string.IsNullOrEmpty(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ItemsResponse>();
        return result?.Items ?? [];
    }

    public async Task<Item> GetItemAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/items/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new ApiException("Item not found");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Item>()
            ?? throw new ApiException("Empty response");
    }

    public async Task<Item> CreateItemAsync(CreateItemRequest request)
    {
        var req = await AuthRequest(HttpMethod.Post, "/items");
        req.Content = JsonContent.Create(request);
        return await SendAsync<Item>(req);
    }

    public async Task<Item> UpdateItemAsync(int id, UpdateItemRequest request)
    {
        var req = await AuthRequest(HttpMethod.Put, $"/items/{id}");
        req.Content = JsonContent.Create(request);
        return await SendAsync<Item>(req);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("/categories");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CategoriesResponse>();
        return result?.Categories ?? [];
    }

    //  Rentals 
    public async Task<Rental> RequestRentalAsync(int itemId,
        DateTime startDate, DateTime endDate)
    {
        var req = await AuthRequest(HttpMethod.Post, "/rentals");
        req.Content = JsonContent.Create(new
        {
            itemId,
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate   = endDate.ToString("yyyy-MM-dd")
        });
        return await SendAsync<Rental>(req);
    }

    public async Task<List<Rental>> GetIncomingRentalsAsync(string? status = null)
    {
        var url = "/rentals/incoming";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        var req = await AuthRequest(HttpMethod.Get, url);
        var result = await SendAsync<RentalsResponse>(req);
        return result.Rentals;
    }

    public async Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null)
    {
        var url = "/rentals/outgoing";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        var req = await AuthRequest(HttpMethod.Get, url);
        var result = await SendAsync<RentalsResponse>(req);
        return result.Rentals;
    }

    public async Task<Rental> GetRentalAsync(int id)
    {
        var req = await AuthRequest(HttpMethod.Get, $"/rentals/{id}");
        return await SendAsync<Rental>(req);
    }

    public async Task UpdateRentalStatusAsync(int rentalId, string status)
    {
        var req = await AuthRequest(HttpMethod.Patch, $"/rentals/{rentalId}/status");
        req.Content = JsonContent.Create(new { status });
        await SendAsync<object>(req);
    }

    // Reviews 
    public async Task<Review> CreateReviewAsync(int rentalId, int rating, string comment)
    {
        var req = await AuthRequest(HttpMethod.Post, "/reviews");
        req.Content = JsonContent.Create(new { rentalId, rating, comment });
        return await SendAsync<Review>(req);
    }

    public async Task<List<Review>> GetItemReviewsAsync(int itemId)
    {
        var response = await _httpClient.GetAsync($"/items/{itemId}/reviews");
        response.EnsureSuccessStatusCode();
        var result = await response.Content
            .ReadFromJsonAsync<ReviewsResponse>();
        return result?.Reviews ?? [];
    }
}