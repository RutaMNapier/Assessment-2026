using RentalApp.ViewModels;

namespace RentalApp.Views;

// code behind for the create and edit item page
public partial class CreateItemPage : ContentPage
{
    private readonly CreateItemViewModel _vm;

    // sets the view model when the page is created
    public CreateItemPage(CreateItemViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // runs every time the page appears
    // loads categories and item data if editing
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCategoriesCommand.ExecuteAsync(null);

        // only load item data when editing an existing item
        if (_vm.IsEditMode)
            await _vm.LoadItemCommand.ExecuteAsync(null);
    }
}