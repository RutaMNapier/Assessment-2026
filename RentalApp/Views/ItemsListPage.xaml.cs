using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class ItemsListPage : ContentPage
{
    public ItemsListPage(ItemsListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}