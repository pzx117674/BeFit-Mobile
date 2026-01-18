using BeFit.Mobile.Models;
using BeFit.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Mobile.ViewModels;

/// <summary>
/// ViewModel dla szczegółów/edycji typu ćwiczenia
/// </summary>
[QueryProperty(nameof(ExerciseTypeId), "id")]
public partial class ExerciseTypeDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private int exerciseTypeId;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string pageTitle = "Nowy typ ćwiczenia";

    [ObservableProperty]
    private string? nameError;

    [ObservableProperty]
    private bool isEditing;

    public ExerciseTypeDetailViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    /// <summary>
    /// Ładuje dane typu ćwiczenia gdy ustawione jest ID
    /// </summary>
    partial void OnExerciseTypeIdChanged(int value)
    {
        if (value > 0)
        {
            IsEditing = true;
            PageTitle = "Edytuj typ ćwiczenia";
            LoadExerciseTypeCommand.Execute(null);
        }
    }

    /// <summary>
    /// Ładuje dane typu ćwiczenia z bazy danych
    /// </summary>
    [RelayCommand]
    private async Task LoadExerciseTypeAsync()
    {
        try
        {
            var exerciseType = await _databaseService.GetExerciseTypeAsync(ExerciseTypeId);
            if (exerciseType != null)
            {
                Name = exerciseType.Name;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować typu ćwiczenia: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Waliduje i zapisuje typ ćwiczenia
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Walidacja
        if (!ValidateInput())
            return;

        try
        {
            var exerciseType = new ExerciseType
            {
                Id = ExerciseTypeId,
                Name = Name.Trim()
            };

            await _databaseService.SaveExerciseTypeAsync(exerciseType);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zapisać typu ćwiczenia: {ex.Message}", "OK");
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
        NameError = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            NameError = "Nazwa ćwiczenia jest wymagana";
            return false;
        }

        if (Name.Length > 80)
        {
            NameError = "Nazwa ćwiczenia może mieć maksymalnie 80 znaków";
            return false;
        }

        return true;
    }
}
