using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using System.Collections.ObjectModel;
using RentalApp.Services;
using RentalApp.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _items;
    private readonly IApiService _api;

    public CreateItemViewModel(IItemRepository items, IApiService api)
    {
        _items = items;
        _api   = api;
    }

    [ObservableProperty] private int itemId;   // 0 = create, >0 = edit
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private decimal dailyRate;
    [ObservableProperty] private int selectedCategoryId;
    [ObservableProperty] private double latitude;
    [ObservableProperty] private double longitude;
    [ObservableProperty] private bool isAvailable = true;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isEditMode;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private ObservableCollection<Category> categories = [];

    partial void OnItemIdChanged(int value)
    {
        IsEditMode = value > 0;
        if (IsEditMode) LoadItemCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadItemAsync()
    {
        var item = await _items.GetByIdAsync(ItemId);
        if (item is null) return;
        Title              = item.Title;
        Description        = item.Description;
        DailyRate          = item.DailyRate;
        SelectedCategoryId = item.CategoryId;
        Latitude           = item.Latitude;
        Longitude          = item.Longitude;
        IsAvailable        = item.IsAvailable;
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var cats = await _api.GetCategoriesAsync();
        Categories = new ObservableCollection<Category>(cats);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) || DailyRate <= 0)
        {
            ErrorMessage = "Title and a positive daily rate are required.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            if (IsEditMode)
            {
                await _items.UpdateAsync(new Item
                {
                    Id          = ItemId,
                    Title       = Title,
                    Description = Description,
                    DailyRate   = DailyRate,
                    IsAvailable = IsAvailable
                });
            }
            else
            {
                await _items.CreateAsync(new Item
                {
                    Title       = Title,
                    Description = Description,
                    DailyRate   = DailyRate,
                    CategoryId  = SelectedCategoryId,
                    Latitude    = Latitude,
                    Longitude   = Longitude
                });
            }
            await Shell.Current.GoToAsync("..");
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
}
