using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Views;

/// @brief View model for the Items List page
/// @details loads items from the API and supports searching by keyword
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _items;

    /// @brief initialises a new instance of ItemsListViewModel
    /// @param items the item repository for loading items
    public ItemsListViewModel(IItemRepository items) => _items = items;

    /// @brief the list of items shown on the page
    [ObservableProperty] private ObservableCollection<Item> items = [];

    /// @brief whether items are loading
    [ObservableProperty] private bool isBusy;

    /// @brief error message shown if loading fails
    [ObservableProperty] private string? errorMessage;

    /// @brief the text typed in the search bar
    [ObservableProperty] private string? searchText;

    /// @brief loads items from the API filtered by search text
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var result = await _items.SearchAsync(null, SearchText, 1, 20);
            Items = new ObservableCollection<Item>(result);
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// @brief navigates to the item detail page
    /// @param item the item the user clcked on
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task GoToDetailAsync(Item item)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(ItemDetailPage)}?id={item.Id}");
    }

    /// @brief navigates to the create item page
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await Shell.Current.GoToAsync(nameof(CreateItemPage));
    }
}