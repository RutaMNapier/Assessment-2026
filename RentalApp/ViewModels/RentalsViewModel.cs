using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

/// @brief View model for the Rentals page
/// @details loads incoming and outgoing rentals and allows toggling between the two views
public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalService _rentalService;

    /// @brief initializes a new instance of RentalsViewModel
    /// @param rentalService the rental service for loading rentals
    public RentalsViewModel(IRentalService rentalService) =>
        _rentalService = rentalService;

    /// @brief rentals where the current user is the owner
    [ObservableProperty] private ObservableCollection<Rental> incoming = [];

    /// @brief rentals where the current user is the borrower
    [ObservableProperty] private ObservableCollection<Rental> outgoing = [];

    /// @brief whether rentals are loading
    [ObservableProperty] private bool isBusy;

    /// @brief true shows incoming, false shows outgoing
    [ObservableProperty] private bool showIncoming = true;

    /// @brief error message shown if loading fails
    [ObservableProperty] private string? errorMessage;

    /// @brief loads both incoming and outgoing rentals from the service
    /// @return a task representing the async operation
    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
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

    /// @brief switches between incoming and outgoing views
    [RelayCommand]
    private void ToggleView() => ShowIncoming = !ShowIncoming;
}