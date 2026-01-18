using SQLite;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Mobile.Models;

/// <summary>
/// Model reprezentujący sesję treningową
/// </summary>
public class TrainingSession : IValidatableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
    [Display(Name = "Start treningu", Description = "Data i czas rozpoczęcia sesji treningowej")]
    public DateTime StartedAt { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Data zakończenia jest wymagana")]
    [Display(Name = "Koniec treningu", Description = "Data i czas zakończenia sesji treningowej")]
    public DateTime EndedAt { get; set; } = DateTime.Now.AddHours(1);

    /// <summary>
    /// Walidacja niestandardowa - sprawdza czy data zakończenia jest późniejsza niż data rozpoczęcia
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndedAt <= StartedAt)
        {
            yield return new ValidationResult(
                "Data zakończenia musi być późniejsza niż data rozpoczęcia.",
                new[] { nameof(EndedAt) });
        }
    }

    /// <summary>
    /// Czas trwania sesji
    /// </summary>
    [Ignore]
    public TimeSpan Duration => EndedAt - StartedAt;

    /// <summary>
    /// Formatowany opis sesji
    /// </summary>
    [Ignore]
    public string DisplayText => $"{StartedAt:dd.MM.yyyy HH:mm} - {EndedAt:HH:mm}";

    public override string ToString() => DisplayText;
}
