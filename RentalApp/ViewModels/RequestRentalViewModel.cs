using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
[QueryProperty(nameof(ItemTitle), "itemTitle")]
[QueryProperty(nameof(DailyRate), "dailyRate")]
public partial class RequestRentalViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    public RequestRentalViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
        StartDate = DateTime.Today.AddDays(1);
        EndDate = DateTime.Today.AddDays(2);
    }

    [ObservableProperty] private int itemId;
    [ObservableProperty] private string itemTitle = string.Empty;
    [ObservableProperty] private decimal dailyRate;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? errorMessage;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private DateTime endDate;

    public DateTime Today => DateTime.Today;

    public decimal TotalPrice
    {
        get
        {
            var days = (EndDate - StartDate).Days;
            return days > 0 ? days * DailyRate : 0;
        }
    }

    partial void OnStartDateChanged(DateTime value) =>
        OnPropertyChanged(nameof(TotalPrice));

    partial void OnEndDateChanged(DateTime value) =>
        OnPropertyChanged(nameof(TotalPrice));

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
            await Shell.Current.DisplayAlert(
                "Success",
                "Rental request submitted successfully!",
                "OK");
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