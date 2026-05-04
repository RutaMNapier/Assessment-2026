using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using RentalApp.Views;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "id")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _items;
    private readonly IApiService _api;

    public ItemDetailViewModel(IItemRepository items, IApiService api)
    {
        _items = items;
        _api   = api;
    }

    [ObservableProperty] private int itemId;
    [ObservableProperty] private Item? item;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isOwner;
    [ObservableProperty] private string? errorMessage;

    partial void OnItemIdChanged(int value) =>
        LoadItemCommand.Execute(null);

    [RelayCommand]
    private async Task LoadItemAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            Item = await _items.GetByIdAsync(ItemId);
            var currentUser = await _api.GetCurrentUserAsync();
            IsOwner = Item?.OwnerId == currentUser?.Id;
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

    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(CreateItemPage)}?itemId={ItemId}");
    }

    [RelayCommand(CanExecute = nameof(IsOwner))]
    private async Task EditItemAsync()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(CreateItemPage)}?id={ItemId}");
    }
}