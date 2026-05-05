using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class RequestRentalPage : ContentPage
{
    public RequestRentalPage(RequestRentalViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}