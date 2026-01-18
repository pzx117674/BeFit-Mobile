using SQLite;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Mobile.Models;

/// <summary>
/// Model reprezentujący wykonane ćwiczenie w ramach sesji treningowej
/// </summary>
public class TrainingEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Required(ErrorMessage = "Sesja treningowa jest wymagana")]
    [Display(Name = "Sesja treningowa", Description = "Wybierz sesję treningową")]
    [Indexed]
    public int TrainingSessionId { get; set; }

    [Required(ErrorMessage = "Typ ćwiczenia jest wymagany")]
    [Display(Name = "Typ ćwiczenia", Description = "Wybierz typ ćwiczenia")]
    [Indexed]
    public int ExerciseTypeId { get; set; }

    [Required(ErrorMessage = "Obciążenie jest wymagane")]
    [Range(0, 1000, ErrorMessage = "Obciążenie musi być w zakresie od 0 do 1000 kg")]
    [Display(Name = "Obciążenie (kg)", Description = "Użyte obciążenie w kilogramach")]
    public double Weight { get; set; }

    [Required(ErrorMessage = "Liczba serii jest wymagana")]
    [Range(1, 100, ErrorMessage = "Liczba serii musi być w zakresie od 1 do 100")]
    [Display(Name = "Liczba serii", Description = "Ile serii zostało wykonanych")]
    public int Sets { get; set; } = 1;

    [Required(ErrorMessage = "Liczba powtórzeń jest wymagana")]
    [Range(1, 100, ErrorMessage = "Liczba powtórzeń musi być w zakresie od 1 do 100")]
    [Display(Name = "Liczba powtórzeń w serii", Description = "Ile powtórzeń w każdej serii")]
    public int Repetitions { get; set; } = 1;

    // Navigation properties (not stored in SQLite, populated manually)
    [Ignore]
    public TrainingSession? TrainingSession { get; set; }

    [Ignore]
    public ExerciseType? ExerciseType { get; set; }

    /// <summary>
    /// Formatowany opis wpisu
    /// </summary>
    [Ignore]
    public string DisplayText => ExerciseType != null 
        ? $"{ExerciseType.Name}: {Sets}x{Repetitions} @ {Weight}kg" 
        : $"Ćwiczenie #{ExerciseTypeId}: {Sets}x{Repetitions} @ {Weight}kg";

    public override string ToString() => DisplayText;
}
