using RentalApp.ViewModels;

namespace RentalApp.Views;

public partial class CreateItemPage : ContentPage
{
    private readonly CreateItemViewModel _vm;

    public CreateItemPage(CreateItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
{
    base.OnAppearing();
    await _vm.LoadCategoriesCommand.ExecuteAsync(null);
    if (_vm.IsEditMode)
        await _vm.LoadItemCommand.ExecuteAsync(null);
}
}