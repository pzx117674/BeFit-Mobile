using BeFit.Mobile.Models;
using BeFit.Mobile.Services;
using BeFit.Mobile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BeFit.Mobile.ViewModels;

/// <summary>
/// ViewModel dla listy wpisów treningowych
/// </summary>
public partial class TrainingEntriesViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<TrainingEntry> trainingEntries = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isEmpty;

    public TrainingEntriesViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Ładuje listę wpisów treningowych z bazy danych
    /// </summary>
    [RelayCommand]
    private async Task LoadTrainingEntriesAsync()
    {
        try
        {
            IsRefreshing = true;
            var items = await _databaseService.GetTrainingEntriesAsync();
            TrainingEntries.Clear();
            foreach (var item in items)
            {
                TrainingEntries.Add(item);
            }
            IsEmpty = TrainingEntries.Count == 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować wpisów treningowych: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Nawigacja do strony tworzenia nowego wpisu treningowego
    /// </summary>
    [RelayCommand]
    private async Task AddTrainingEntryAsync()
    {
        // Sprawdź czy są dostępne sesje i typy ćwiczeń
        var sessions = await _databaseService.GetTrainingSessionsAsync();
        var exerciseTypes = await _databaseService.GetExerciseTypesAsync();

        if (sessions.Count == 0)
        {
            await Shell.Current.DisplayAlert("Brak danych", 
                "Najpierw utwórz sesję treningową.", "OK");
            return;
        }

        if (exerciseTypes.Count == 0)
        {
            await Shell.Current.DisplayAlert("Brak danych", 
                "Najpierw dodaj typy ćwiczeń.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(TrainingEntryDetailPage));
    }

    /// <summary>
    /// Nawigacja do strony edycji wybranego wpisu treningowego
    /// </summary>
    [RelayCommand]
    private async Task EditTrainingEntryAsync(TrainingEntry entry)
    {
        if (entry == null) return;
        await Shell.Current.GoToAsync($"{nameof(TrainingEntryDetailPage)}?id={entry.Id}");
    }

    /// <summary>
    /// Usuwa wybrany wpis treningowy
    /// </summary>
    [RelayCommand]
    private async Task DeleteTrainingEntryAsync(TrainingEntry entry)
    {
        if (entry == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", 
            $"Czy na pewno chcesz usunąć wpis \"{entry.DisplayText}\"?", "Tak", "Nie");
        
        if (confirm)
        {
            await _databaseService.DeleteTrainingEntryAsync(entry);
            TrainingEntries.Remove(entry);
            IsEmpty = TrainingEntries.Count == 0;
        }
    }
}
