using RentalApp.Views;

namespace RentalApp;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
        Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
        Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
        Routing.RegisterRoute(nameof(Views.UserListPage), typeof(Views.UserListPage));
        Routing.RegisterRoute(nameof(Views.UserDetailPage), typeof(Views.UserDetailPage));
        Routing.RegisterRoute(nameof(Views.TempPage), typeof(Views.TempPage));
        Routing.RegisterRoute(nameof(Views.ItemDetailPage), typeof(Views.ItemDetailPage));
        Routing.RegisterRoute(nameof(Views.CreateItemPage), typeof(Views.CreateItemPage));
        Routing.RegisterRoute(nameof(Views.RentalsPage), typeof(Views.RentalsPage));
        Routing.RegisterRoute(nameof(Views.RequestRentalPage), typeof(Views.RequestRentalPage));
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Check for stored JWT token
        var token = SecureStorage.Default.GetAsync("jwt_token").GetAwaiter().GetResult();

        if (string.IsNullOrEmpty(token))
        {
            // Not logged in — show login page
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            return new Window(new NavigationPage(loginPage));
        }

        // Already logged in — go straight to Shell
        var shell = _serviceProvider.GetService<AppShell>()
            ?? throw new InvalidOperationException("AppShell could not be resolved.");

        return new Window(shell);
    }
}