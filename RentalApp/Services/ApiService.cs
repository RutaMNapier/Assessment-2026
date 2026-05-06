namespace RentalApp.Services;

using RentalApp.Database.Models;
using System.Net.Http.Json;

// http client implementation of IApiService
// all requests to the shared REST API go through this class
// authenticated endpoints use AuthRequest() to inject Bearer token
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) => _httpClient = httpClient;

    // generic GET helper unauthenticated
    public async Task<T?> GetAsync<T>(string url) =>
        await _httpClient.GetFromJsonAsync<T>(url);

    // generic POST helper unauthenticated
    public async Task<HttpResponseMessage> PostAsync<T>(string url, T data) =>
        await _httpClient.PostAsJsonAsync(url, data);

    // builds authenticated request by reading JWT from SecureStorage
    // used by all endpoints that require a logged-in user
    private async Task<HttpRequestMessage> AuthRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        var token = await SecureStorage.Default.GetAsync("jwt_token");
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    // sends a request and deserialises the response
    // throws ApiException if response is not successful
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

    // auth 

    // POST /auth/token login and get JWT
    public async Task<AuthToken> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/token", new { email, password });
        if (!response.IsSuccessStatusCode)
            throw new ApiException("Invalid email or password");
        return await response.Content.ReadFromJsonAsync<AuthToken>()
            ?? throw new ApiException("Empty response");
    }

    // POST /auth/register create new user account
    public async Task<User> RegisterAsync(
        string firstName, string lastName, string email, string password)
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

    // GET /users/me get current authenticated user profile
    public async Task<User> GetCurrentUserAsync()
    {
        var req = await AuthRequest(HttpMethod.Get, "/users/me");
        return await SendAsync<User>(req);
    }

    // items 

    // GET /items get paginated list with optional category and search filters
    public async Task<List<Item>> GetItemsAsync(
        string? category = null, string? search = null, int page = 1)
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

    // GET /items/{id} get single item by id
    public async Task<Item> GetItemAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/items/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new ApiException("Item not found");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Item>()
            ?? throw new ApiException("Empty response");
    }

    // POST /items create new item listing, requires auth
    public async Task<Item> CreateItemAsync(CreateItemRequest request)
    {
        var req = await AuthRequest(HttpMethod.Post, "/items");
        req.Content = JsonContent.Create(request);
        return await SendAsync<Item>(req);
    }

    // PUT /items/{id} update item, api enforces owner only
    public async Task<Item> UpdateItemAsync(int id, UpdateItemRequest request)
    {
        var req = await AuthRequest(HttpMethod.Put, $"/items/{id}");
        req.Content = JsonContent.Create(request);
        return await SendAsync<Item>(req);
    }

    // GET /categories get all available categories
    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _httpClient.GetAsync("/categories");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CategoriesResponse>();
        return result?.Categories ?? [];
    }

    // rentals 

    // POST /rentals submit a rental request, requires auth
    public async Task<Rental> RequestRentalAsync(
        int itemId, DateTime startDate, DateTime endDate)
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

    // GET /rentals/incoming rentals where current user is the owner
    public async Task<List<Rental>> GetIncomingRentalsAsync(string? status = null)
    {
        var url = "/rentals/incoming";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        var req = await AuthRequest(HttpMethod.Get, url);
        var result = await SendAsync<RentalsResponse>(req);
        return result.Rentals;
    }

    // GET /rentals/outgoing rentals where current user is the borrower
    public async Task<List<Rental>> GetOutgoingRentalsAsync(string? status = null)
    {
        var url = "/rentals/outgoing";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        var req = await AuthRequest(HttpMethod.Get, url);
        var result = await SendAsync<RentalsResponse>(req);
        return result.Rentals;
    }

    // GET /rentals/{id} get single rental by id
    public async Task<Rental> GetRentalAsync(int id)
    {
        var req = await AuthRequest(HttpMethod.Get, $"/rentals/{id}");
        return await SendAsync<Rental>(req);
    }

    // PATCH /rentals/{id}/status update rental status (approved, rejected, returned)
    public async Task UpdateRentalStatusAsync(int rentalId, string status)
    {
        var req = await AuthRequest(HttpMethod.Patch, $"/rentals/{rentalId}/status");
        req.Content = JsonContent.Create(new { status });
        await SendAsync<object>(req);
    }

    // reviews 

    // POST /reviews submit a review after rental completion
    public async Task<Review> CreateReviewAsync(int rentalId, int rating, string comment)
    {
        var req = await AuthRequest(HttpMethod.Post, "/reviews");
        req.Content = JsonContent.Create(new { rentalId, rating, comment });
        return await SendAsync<Review>(req);
    }

    // GET /items/{id}/reviews get all reviews for an item
    public async Task<List<Review>> GetItemReviewsAsync(int itemId)
    {
        var response = await _httpClient.GetAsync($"/items/{itemId}/reviews");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ReviewsResponse>();
        return result?.Reviews ?? [];
    }
}