namespace RentalApp.Services;

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToAsync(string route, Dictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

   public async Task NavigateBackAsync()
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("..");
        else
            await Application.Current!.Windows[0].Page!.Navigation.PopAsync();
    }

    public async Task NavigateToRootAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }

    public async Task PopToRootAsync()
    {
        await Shell.Current.Navigation.PopToRootAsync();
    }
}