using RentalApp.ViewModels;

namespace RentalApp.Views;

// code behind for register page
public partial class RegisterPage : ContentPage
{
    // sets the view model when page is created
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}