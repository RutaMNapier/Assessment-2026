using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class ItemsListPage : ContentPage
{
    private readonly ItemsListViewModel _vm;

    public ItemsListPage(ItemsListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadItemsCommand.Execute(null);
    }
}