using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseStatistics
    {
        [Display(Name = "Typ ćwiczenia")]
        public string ExerciseTypeName { get; set; } = string.Empty;

        [Display(Name = "Liczba wykonanych treningów")]
        public int TimesPerformed { get; set; }

        [Display(Name = "Łączna liczba powtórzeń")]
        public int TotalRepetitions { get; set; }

        [Display(Name = "Średnie obciążenie (kg)")]
        public double AverageWeight { get; set; }

        [Display(Name = "Maksymalne obciążenie (kg)")]
        public double MaxWeight { get; set; }
    }
}

