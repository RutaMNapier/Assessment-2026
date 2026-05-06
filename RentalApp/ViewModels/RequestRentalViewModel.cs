using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

/// @brief View model for the Request Rental page
/// @details receives item details via query properties, lets the user pick dates
/// and calculates the total price before submitting the rental request
[QueryProperty(nameof(ItemId),    "itemId")]
[QueryProperty(nameof(ItemTitle), "itemTitle")]
[QueryProperty(nameof(DailyRate), "dailyRate")]
public partial class RequestRentalViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    /// @brief initializes a new instance of RequestRentalViewModel
    /// @param rentalService the rental service for submitting requests
    public RequestRentalViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;

        // default to tomorrow and the day after
        StartDate = DateTime.Today.AddDays(1);
        EndDate   = DateTime.Today.AddDays(2);
    }

    /// @brief the id of the item being rented
    [ObservableProperty] private int itemId;

    /// @brief the title of the item shown at the top of the page
    [ObservableProperty] private string itemTitle = string.Empty;

    /// @brief the daily rental rate used to calculate the total price
    [ObservableProperty] private decimal dailyRate;

    /// @brief whether the request is being submitted
    [ObservableProperty] private bool isBusy;

    /// @brief error message shown if something goes wrong
    [ObservableProperty] private string? errorMessage;

    /// @brief the date the rental starts
    [ObservableProperty] private DateTime startDate;

    /// @brief the date the rental ends
    [ObservableProperty] private DateTime endDate;

    /// @brief today's date used as the minimum selectable date
    public DateTime Today => DateTime.Today;

    /// @brief the total cost calculated from the number of days and daily rate
    public decimal TotalPrice
    {
        get
        {
            var days = (EndDate - StartDate).Days;
            return days > 0 ? days * DailyRate : 0;
        }
    }

    /// @brief recalculates total price when start date changes
    partial void OnStartDateChanged(DateTime value) =>
        OnPropertyChanged(nameof(TotalPrice));

    /// @brief recalculates total price when end date changes
    partial void OnEndDateChanged(DateTime value) =>
        OnPropertyChanged(nameof(TotalPrice));

    /// @brief validates the dates and submits the rental request
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (EndDate <= StartDate)
        {
            ErrorMessage = "End date must be after start date.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        try
        {
            await _rentalService.RequestRentalAsync(ItemId, StartDate, EndDate);
            await Shell.Current.GoToAsync("//BrowseItems");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}