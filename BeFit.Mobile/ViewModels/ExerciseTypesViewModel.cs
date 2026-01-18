using BeFit.Mobile.Models;
using BeFit.Mobile.Services;
using BeFit.Mobile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BeFit.Mobile.ViewModels;

/// <summary>
/// ViewModel dla listy typów ćwiczeń
/// </summary>
public partial class ExerciseTypesViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<ExerciseType> exerciseTypes = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isEmpty;

    public ExerciseTypesViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Ładuje listę typów ćwiczeń z bazy danych
    /// </summary>
    [RelayCommand]
    private async Task LoadExerciseTypesAsync()
    {
        try
        {
            IsRefreshing = true;
            var items = await _databaseService.GetExerciseTypesAsync();
            ExerciseTypes.Clear();
            foreach (var item in items)
            {
                ExerciseTypes.Add(item);
            }
            IsEmpty = ExerciseTypes.Count == 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować typów ćwiczeń: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Nawigacja do strony tworzenia nowego typu ćwiczenia
    /// </summary>
    [RelayCommand]
    private async Task AddExerciseTypeAsync()
    {
        await Shell.Current.GoToAsync(nameof(ExerciseTypeDetailPage));
    }

    /// <summary>
    /// Nawigacja do strony edycji wybranego typu ćwiczenia
    /// </summary>
    [RelayCommand]
    private async Task EditExerciseTypeAsync(ExerciseType exerciseType)
    {
        if (exerciseType == null) return;
        await Shell.Current.GoToAsync($"{nameof(ExerciseTypeDetailPage)}?id={exerciseType.Id}");
    }

    /// <summary>
    /// Usuwa wybrany typ ćwiczenia
    /// </summary>
    [RelayCommand]
    private async Task DeleteExerciseTypeAsync(ExerciseType exerciseType)
    {
        if (exerciseType == null) return;

        // Sprawdź czy typ ćwiczenia jest używany
        if (await _databaseService.IsExerciseTypeInUseAsync(exerciseType.Id))
        {
            await Shell.Current.DisplayAlert("Błąd", 
                "Nie można usunąć typu ćwiczenia, który jest używany w wpisach treningowych.", "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", 
            $"Czy na pewno chcesz usunąć typ ćwiczenia \"{exerciseType.Name}\"?", "Tak", "Nie");
        
        if (confirm)
        {
            await _databaseService.DeleteExerciseTypeAsync(exerciseType);
            ExerciseTypes.Remove(exerciseType);
            IsEmpty = ExerciseTypes.Count == 0;
        }
    }
}
