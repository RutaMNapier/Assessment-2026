using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class RentalsPage : ContentPage
{
    public RentalsPage(RentalsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}