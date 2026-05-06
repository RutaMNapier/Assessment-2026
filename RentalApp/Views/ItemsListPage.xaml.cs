using RentalApp.ViewModels;

namespace RentalApp.Views;

// code behind for the browse items page
public partial class ItemsListPage : ContentPage
{
    private readonly ItemsListViewModel _vm;

    // sets the view model when the page is created
    public ItemsListPage(ItemsListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // loads items every time the page appears
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadItemsCommand.Execute(null);
    }
}