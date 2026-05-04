using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;

public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalRepository _rentals;

    public RentalsViewModel(IRentalRepository rentals) => _rentals = rentals;

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
            var inc = await _rentals.GetIncomingAsync();
            var out_ = await _rentals.GetOutgoingAsync();
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