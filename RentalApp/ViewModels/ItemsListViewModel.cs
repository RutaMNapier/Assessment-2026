using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Views;

public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _items;

    public ItemsListViewModel(IItemRepository items) => _items = items;

    [ObservableProperty] private ObservableCollection<Item> items = [];
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private string? searchText;

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

    [RelayCommand]
    private async Task GoToDetailAsync(Item item)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(ItemDetailPage)}?id={item.Id}");
    }

    [RelayCommand]
    private async Task GoToCreateAsync()
    {
        await Shell.Current.GoToAsync(nameof(CreateItemPage));
    }
}
