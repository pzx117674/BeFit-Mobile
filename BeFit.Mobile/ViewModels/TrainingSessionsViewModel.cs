using BeFit.Mobile.Models;
using BeFit.Mobile.Services;
using BeFit.Mobile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BeFit.Mobile.ViewModels;

/// <summary>
/// ViewModel dla listy sesji treningowych
/// </summary>
public partial class TrainingSessionsViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<TrainingSession> trainingSessions = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isEmpty;

    public TrainingSessionsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Ładuje listę sesji treningowych z bazy danych
    /// </summary>
    [RelayCommand]
    private async Task LoadTrainingSessionsAsync()
    {
        try
        {
            IsRefreshing = true;
            var items = await _databaseService.GetTrainingSessionsAsync();
            TrainingSessions.Clear();
            foreach (var item in items)
            {
                TrainingSessions.Add(item);
            }
            IsEmpty = TrainingSessions.Count == 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować sesji treningowych: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Nawigacja do strony tworzenia nowej sesji treningowej
    /// </summary>
    [RelayCommand]
    private async Task AddTrainingSessionAsync()
    {
        await Shell.Current.GoToAsync(nameof(TrainingSessionDetailPage));
    }

    /// <summary>
    /// Nawigacja do strony edycji wybranej sesji treningowej
    /// </summary>
    [RelayCommand]
    private async Task EditTrainingSessionAsync(TrainingSession session)
    {
        if (session == null) return;
        await Shell.Current.GoToAsync($"{nameof(TrainingSessionDetailPage)}?id={session.Id}");
    }

    /// <summary>
    /// Usuwa wybraną sesję treningową wraz z powiązanymi wpisami
    /// </summary>
    [RelayCommand]
    private async Task DeleteTrainingSessionAsync(TrainingSession session)
    {
        if (session == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", 
            $"Czy na pewno chcesz usunąć sesję treningową z {session.StartedAt:dd.MM.yyyy}?\n\nUwaga: Zostaną również usunięte wszystkie powiązane wpisy treningowe.", 
            "Tak", "Nie");
        
        if (confirm)
        {
            await _databaseService.DeleteTrainingSessionAsync(session);
            TrainingSessions.Remove(session);
            IsEmpty = TrainingSessions.Count == 0;
        }
    }
}
