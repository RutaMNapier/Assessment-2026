using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    public RentalsViewModel(IRentalService rentalService) => _rentalService = rentalService;

    [ObservableProperty] private ObservableCollection<Rental> incoming = [];
    [ObservableProperty] private ObservableCollection<Rental> outgoing = [];
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool showIncoming = true;
    [ObservableProperty] private string? errorMessage;

    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            // Business logic goes through service, not repository directly
            var inc  = await _rentalService.GetIncomingRentalsAsync();
            var out_ = await _rentalService.GetOutgoingRentalsAsync();
            Incoming = new ObservableCollection<Rental>(inc);
            Outgoing = new ObservableCollection<Rental>(out_);
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
    private void ToggleView() => ShowIncoming = !ShowIncoming;
}