using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnBrowseItemsTapped(object sender, EventArgs e)
    {
    await Shell.Current.GoToAsync("//BrowseItems");
    }

    private async void OnMyRentalsTapped(object sender, EventArgs e)
    {
    await Shell.Current.GoToAsync("//Rentals");
    }

    private async void OnListItemTapped(object sender, EventArgs e)
    {
    await Shell.Current.GoToAsync("//ListAnItem");
    }
}