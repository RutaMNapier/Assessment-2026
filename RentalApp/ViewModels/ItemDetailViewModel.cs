using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.Views;

namespace RentalApp.ViewModels;

/// @brief View model for the Item Detail page
/// @details loads the item from the API and checks if the current user is the owner
/// shows request rental button for borrowers and edit button for owners
[QueryProperty(nameof(ItemId), "id")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _items;
    private readonly IApiService _api;

    /// @brief initializes a new instance of ItemDetailViewModel
    /// @param items the item repository for loading item data
    /// @param api the api service for getting the current user
    public ItemDetailViewModel(IItemRepository items, IApiService api)
    {
        _items = items;
        _api   = api;
    }

    /// @brief the id of the item to display
    [ObservableProperty] private int itemId;

    /// @brief the item loaded from the API
    [ObservableProperty] private Item? item;

    /// @brief whether data is loading
    [ObservableProperty] private bool isBusy;

    /// @brief true if the current user owns this item
    [ObservableProperty] private bool isOwner;

    /// @brief error message shown if loading fails
    [ObservableProperty] private string? errorMessage;

    /// @brief loads the item automatically when the id is set via navigation
    partial void OnItemIdChanged(int value) =>
        LoadItemCommand.Execute(null);

    /// @brief loads the item from the API and checks ownership
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task LoadItemAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            Item = await _items.GetByIdAsync(ItemId);

            // check if the logged in user owns this item
            var currentUser = await _api.GetCurrentUserAsync();
            IsOwner = Item?.OwnerId == currentUser?.Id;

            // refresh the edit button state after ownership is known
            EditItemCommand.NotifyCanExecuteChanged();
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

    /// @brief navigates to the request rental page with item details
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(RequestRentalPage)}?itemId={ItemId}" +
            $"&itemTitle={Uri.EscapeDataString(Item?.Title ?? string.Empty)}" +
            $"&dailyRate={Item?.DailyRate}");
    }

    /// @brief only the item owner can edit the item
    private bool CanEditItem() => IsOwner;

    /// @brief navigates to the edit item page
    /// @details only available when CanEditItem returns true
    /// @return a task representing the async operation
    [RelayCommand(CanExecute = nameof(CanEditItem))]
    private async Task EditItemAsync()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(CreateItemPage)}?id={ItemId}");
    }
}