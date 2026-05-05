namespace RentalApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using System.Collections.ObjectModel;
using RentalApp.Services;

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

    [ObservableProperty] private int itemId;
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private decimal dailyRate;
    [ObservableProperty] private double latitude;
    [ObservableProperty] private double longitude;
    [ObservableProperty] private bool isAvailable = true;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isEditMode;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private ObservableCollection<Category> categories = [];
    [ObservableProperty] private Category? selectedCategory;

    partial void OnItemIdChanged(int value)
    {
        IsEditMode = value > 0;
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var cats = await _api.GetCategoriesAsync();
        Categories = new ObservableCollection<Category>(cats);
    }

    [RelayCommand]
    private async Task LoadItemAsync()
    {
        var item = await _items.GetByIdAsync(ItemId);
        if (item is null) return;
        Title            = item.Title;
        Description      = item.Description;
        DailyRate        = item.DailyRate;
        Latitude         = item.Latitude ?? 0;    
        Longitude        = item.Longitude ?? 0;   
        IsAvailable      = item.IsAvailable;
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == item.CategoryId);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
{
    ErrorMessage = "Title is required.";
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
                    CategoryId  = SelectedCategory?.Id ?? 0,
                    Latitude    = Latitude,
                    Longitude   = Longitude,
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
                    CategoryId  = SelectedCategory?.Id ?? 0,
                    Latitude    = Latitude,
                    Longitude   = Longitude
                });
            }
            await Shell.Current.GoToAsync("//BrowseItems");
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