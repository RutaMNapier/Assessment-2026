using RentalApp.ViewModels;
namespace RentalApp.Views;

public partial class CreateItemPage : ContentPage
{
    public CreateItemPage(CreateItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}