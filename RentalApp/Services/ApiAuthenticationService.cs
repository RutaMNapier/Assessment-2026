using System.Net.Http.Headers;
using System.Net.Http.Json;
using RentalApp.Database.Models;

namespace RentalApp.Services;

// API implementation of IAuthenticationService
// communicates with the shared REST API for authentication
// swappable with local AuthenticationService via MauiProgram.cs
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    private readonly List<string> _currentUserRoles = new();

    // stores the expiry time of the JWT token returned by the API
    private DateTime _tokenExpiresAt;

    // checks if the stored token has expired
    private bool IsTokenExpired() => DateTime.UtcNow >= _tokenExpiresAt;

    public event EventHandler<bool>? AuthenticationStateChanged;

    // user is authenticated if: user exists and token has not expired
    public bool IsAuthenticated => _currentUser != null && !IsTokenExpired();

    // prevents access to user data if token is expired
    public User? CurrentUser => IsAuthenticated ? _currentUser : null;
    public List<string> CurrentUserRoles => _currentUserRoles;

    public ApiAuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // POST /auth/token authenticates user and stores JWT
    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/token", new { email, password });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return new AuthenticationResult(false, error?.Message ?? "Login failed");
            }

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

            // save token expiry for IsTokenExpired() check
            _tokenExpiresAt = token!.ExpiresAt;

            // set bearer token on all future requests
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token!.Token);

            // persist token so app can resume session after restart
            await SecureStorage.Default.SetAsync("jwt_token", token!.Token);

            // GET /users/me load current user profile
            var meResponse = await _httpClient.GetAsync("users/me");
            var profile = await meResponse.Content.ReadFromJsonAsync<UserProfileResponse>();

            _currentUser = new User
            {
                Id        = profile!.Id,
                Email     = profile.Email,
                FirstName = profile.FirstName,
                LastName  = profile.LastName,
                CreatedAt = profile.CreatedAt,
                IsActive  = true
            };

            AuthenticationStateChanged?.Invoke(this, true);
            return new AuthenticationResult(true, "Login successful");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, $"Login failed: {ex.Message}");
        }
    }

    // POST /auth/register creates new user account
    public async Task<AuthenticationResult> RegisterAsync(
        string firstName, string lastName, string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", new
            {
                firstName,
                lastName,
                email,
                password
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return new AuthenticationResult(false, error?.Message ?? "Registration failed");
            }

            return new AuthenticationResult(true, "Registration successful. Please log in.");
        }
        catch (Exception ex)
        {
            return new AuthenticationResult(false, $"Registration failed: {ex.Message}");
        }
    }

    // clears all auth state and removes bearer token from http client
    public Task LogoutAsync()
    {
        _currentUser = null;
        _currentUserRoles.Clear();
        _tokenExpiresAt = DateTime.MinValue;  // force IsTokenExpired() to return true
        _httpClient.DefaultRequestHeaders.Authorization = null;
        AuthenticationStateChanged?.Invoke(this, false);
        return Task.CompletedTask;
    }

    // role checks, always false
    public bool HasRole(string roleName) =>
        _currentUserRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);

    public bool HasAnyRole(params string[] roleNames) =>
        roleNames.Any(HasRole);

    public bool HasAllRoles(params string[] roleNames) =>
        roleNames.All(HasRole);

    // password change not supported 
    public Task<bool> ChangePasswordAsync(string currentPassword, string newPassword) =>
        Task.FromResult(false);


    // private DTOs for deserialising API responses 

    private record TokenResponse(string Token, DateTime ExpiresAt, int UserId);

    private record UserProfileResponse(
        int Id, string Email, string FirstName, string LastName, DateTime CreatedAt);

    private record ApiErrorResponse(string Error, string Message);
}