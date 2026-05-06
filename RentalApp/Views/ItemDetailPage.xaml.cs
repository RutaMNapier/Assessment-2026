using RentalApp.ViewModels;

namespace RentalApp.Views;

// code behind for the item detail page
public partial class ItemDetailPage : ContentPage
{
    // sets the view model when the page is created
    public ItemDetailPage(ItemDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}