namespace RentalApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using System.Collections.ObjectModel;
using RentalApp.Services;

/// @brief View model for the Create/Edit Item page
/// @details Handles both creating new items and editing existing ones
/// depending on whether an item id is passed as a query property
[QueryProperty(nameof(ItemId), "id")]
public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _items;
    private readonly IApiService _api;

    /// @brief Initializes a new instance of CreateItemViewModel
    /// @param items the item repository for data access
    /// @param api the api service for loading categories
    public CreateItemViewModel(IItemRepository items, IApiService api)
    {
        _items = items;
        _api   = api;
    }

    /// @brief the id of the item being edited (0 if creating new)
    [ObservableProperty] private int itemId;

    /// @brief the title of the item
    [ObservableProperty] private string title = string.Empty;

    /// @brief the description of the item
    [ObservableProperty] private string description = string.Empty;

    /// @brief the daily rental rate in pounds
    [ObservableProperty] private decimal dailyRate;

    /// @brief latitude of the item location (defaults to Edinburgh)
    [ObservableProperty] private double latitude = 55.9533;

    /// @brief longitude of the item location (defaults to Edinburgh)
    [ObservableProperty] private double longitude = -3.1883;

    /// @brief whether the item is available to rent
    [ObservableProperty] private bool isAvailable = true;

    /// @brief whether a save operation is in progress
    [ObservableProperty] private bool isBusy;

    /// @brief true when editing an existing item, false when creating new
    [ObservableProperty] private bool isEditMode;

    /// @brief error message shown to the user if something goes wrong
    [ObservableProperty] private string? errorMessage;

    /// @brief list of categories shown in the picker
    [ObservableProperty] private ObservableCollection<Category> categories = [];

    /// @brief the category selected by the user
    [ObservableProperty] private Category? selectedCategory;

    /// @brief text version of daily rate to handle decimal input correctly
    /// @details converts the typed string to a decimal, handling both . and , separators
    private string _dailyRateText = string.Empty;
    public string DailyRateText
    {
        get => _dailyRateText;
        set
        {
            _dailyRateText = value;
            OnPropertyChanged();
            // replace comma with dot so 5,00 and 5.00 both work
            var normalised = value.Replace(',', '.');
            DailyRate = decimal.TryParse(normalised,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var d) ? d : 0;
        }
    }

    /// @brief switches to edit mode when an item id is provided
    partial void OnItemIdChanged(int value)
    {
        IsEditMode = value > 0;
    }

    /// @brief loads the list of categories from the API
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var cats = await _api.GetCategoriesAsync();
        Categories = new ObservableCollection<Category>(cats);
    }

    /// @brief loads the existing item details when editing
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task LoadItemAsync()
    {
        var item = await _items.GetByIdAsync(ItemId);
        if (item is null) return;

        Title            = item.Title;
        Description      = item.Description;
        DailyRateText    = item.DailyRate.ToString();
        DailyRate        = item.DailyRate;
        Latitude         = item.Latitude ?? 55.9533;
        Longitude        = item.Longitude ?? -3.1883;
        IsAvailable      = item.IsAvailable;
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == item.CategoryId);
    }

    /// @brief validates the form and saves the item
    /// @details creates a new item or updates an existing one depending on IsEditMode
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Title is required.";
            return;
        }

        if (DailyRate <= 0)
        {
            ErrorMessage = $"Daily rate is {DailyRate} — enter a valid amount like 5 or 5.00";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            if (IsEditMode)
            {
                // update existing item
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
                // create new item
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