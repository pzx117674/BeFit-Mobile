using BeFit.Mobile.Models;
using BeFit.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BeFit.Mobile.ViewModels;

/// <summary>
/// ViewModel dla szczegółów/edycji wpisu treningowego
/// </summary>
[QueryProperty(nameof(EntryId), "id")]
public partial class TrainingEntryDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private int entryId;

    [ObservableProperty]
    private ObservableCollection<TrainingSession> trainingSessions = new();

    [ObservableProperty]
    private ObservableCollection<ExerciseType> exerciseTypes = new();

    [ObservableProperty]
    private TrainingSession? selectedSession;

    [ObservableProperty]
    private ExerciseType? selectedExerciseType;

    [ObservableProperty]
    private double weight;

    [ObservableProperty]
    private int sets = 1;

    [ObservableProperty]
    private int repetitions = 1;

    [ObservableProperty]
    private string pageTitle = "Nowy wpis treningowy";

    [ObservableProperty]
    private string? sessionError;

    [ObservableProperty]
    private string? exerciseTypeError;

    [ObservableProperty]
    private string? weightError;

    [ObservableProperty]
    private string? setsError;

    [ObservableProperty]
    private string? repetitionsError;

    [ObservableProperty]
    private bool isEditing;

    public TrainingEntryDetailViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Ładuje dane wpisu treningowego gdy ustawione jest ID
    /// </summary>
    partial void OnEntryIdChanged(int value)
    {
        if (value > 0)
        {
            IsEditing = true;
            PageTitle = "Edytuj wpis treningowy";
        }
        LoadDataCommand.Execute(null);
    }

    /// <summary>
    /// Ładuje listy wyboru i dane wpisu (jeśli edycja)
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            // Ładuj listy wyboru
            var sessions = await _databaseService.GetTrainingSessionsAsync();
            var types = await _databaseService.GetExerciseTypesAsync();

            TrainingSessions.Clear();
            foreach (var session in sessions)
            {
                TrainingSessions.Add(session);
            }

            ExerciseTypes.Clear();
            foreach (var type in types)
            {
                ExerciseTypes.Add(type);
            }

            // Jeśli edycja, załaduj dane wpisu
            if (EntryId > 0)
            {
                var entry = await _databaseService.GetTrainingEntryAsync(EntryId);
                if (entry != null)
                {
                    SelectedSession = TrainingSessions.FirstOrDefault(s => s.Id == entry.TrainingSessionId);
                    SelectedExerciseType = ExerciseTypes.FirstOrDefault(e => e.Id == entry.ExerciseTypeId);
                    Weight = entry.Weight;
                    Sets = entry.Sets;
                    Repetitions = entry.Repetitions;
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Waliduje i zapisuje wpis treningowy
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Walidacja
        if (!ValidateInput())
            return;

        try
        {
            var entry = new TrainingEntry
            {
                Id = EntryId,
                TrainingSessionId = SelectedSession!.Id,
                ExerciseTypeId = SelectedExerciseType!.Id,
                Weight = Weight,
                Sets = Sets,
                Repetitions = Repetitions
            };

            await _databaseService.SaveTrainingEntryAsync(entry);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zapisać wpisu treningowego: {ex.Message}", "OK");
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
        SessionError = null;
        ExerciseTypeError = null;
        WeightError = null;
        SetsError = null;
        RepetitionsError = null;

        bool isValid = true;

        if (SelectedSession == null)
        {
            SessionError = "Sesja treningowa jest wymagana";
            isValid = false;
        }

        if (SelectedExerciseType == null)
        {
            ExerciseTypeError = "Typ ćwiczenia jest wymagany";
            isValid = false;
        }

        if (Weight < 0 || Weight > 1000)
        {
            WeightError = "Obciążenie musi być w zakresie od 0 do 1000 kg";
            isValid = false;
        }

        if (Sets < 1 || Sets > 100)
        {
            SetsError = "Liczba serii musi być w zakresie od 1 do 100";
            isValid = false;
        }

        if (Repetitions < 1 || Repetitions > 100)
        {
            RepetitionsError = "Liczba powtórzeń musi być w zakresie od 1 do 100";
            isValid = false;
        }

        return isValid;
    }
}
