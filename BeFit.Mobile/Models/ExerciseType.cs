using SQLite;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Mobile.Models;

/// <summary>
/// Model reprezentujący typ ćwiczenia
/// </summary>
public class ExerciseType
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana")]
    [SQLite.MaxLength(80)]
    [Display(Name = "Nazwa ćwiczenia", Description = "Nazwa typu ćwiczenia")]
    public string Name { get; set; } = string.Empty;

    public override string ToString() => Name;
}
