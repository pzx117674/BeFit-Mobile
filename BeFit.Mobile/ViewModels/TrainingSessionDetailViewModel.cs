using BeFit.Mobile.Models;
using BeFit.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeFit.Mobile.ViewModels;

/// <summary>
/// ViewModel dla szczegółów/edycji sesji treningowej
/// </summary>
[QueryProperty(nameof(SessionId), "id")]
public partial class TrainingSessionDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private int sessionId;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan startTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan endTime = DateTime.Now.AddHours(1).TimeOfDay;

    [ObservableProperty]
    private string pageTitle = "Nowa sesja treningowa";

    [ObservableProperty]
    private string? dateError;

    [ObservableProperty]
    private bool isEditing;

    public TrainingSessionDetailViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Ładuje dane sesji treningowej gdy ustawione jest ID
    /// </summary>
    partial void OnSessionIdChanged(int value)
    {
        if (value > 0)
        {
            IsEditing = true;
            PageTitle = "Edytuj sesję treningową";
            LoadTrainingSessionCommand.Execute(null);
        }
    }

    /// <summary>
    /// Ładuje dane sesji treningowej z bazy danych
    /// </summary>
    [RelayCommand]
    private async Task LoadTrainingSessionAsync()
    {
        try
        {
            var session = await _databaseService.GetTrainingSessionAsync(SessionId);
            if (session != null)
            {
                StartDate = session.StartedAt.Date;
                StartTime = session.StartedAt.TimeOfDay;
                EndDate = session.EndedAt.Date;
                EndTime = session.EndedAt.TimeOfDay;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować sesji treningowej: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Waliduje i zapisuje sesję treningową
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Walidacja
        if (!ValidateInput())
            return;

        try
        {
            var session = new TrainingSession
            {
                Id = SessionId,
                StartedAt = StartDate.Add(StartTime),
                EndedAt = EndDate.Add(EndTime)
            };

            await _databaseService.SaveTrainingSessionAsync(session);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zapisać sesji treningowej: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Anuluje edycję i wraca do listy
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Waliduje wprowadzone dane
    /// </summary>
    private bool ValidateInput()
    {
        DateError = null;

        var startDateTime = StartDate.Add(StartTime);
        var endDateTime = EndDate.Add(EndTime);

        if (endDateTime <= startDateTime)
        {
            DateError = "Data zakończenia musi być późniejsza niż data rozpoczęcia";
            return false;
        }

        return true;
    }
}
