using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class TrainingEntryCreateDto
    {
        [Required]
        [Display(Name = "Sesja treningowa", Description = "Wybierz sesję treningową")]
        public int TrainingSessionId { get; set; }

        [Required]
        [Display(Name = "Typ ćwiczenia", Description = "Wybierz typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Obciążenie (kg)")]
        public double Weight { get; set; }

        [Range(1, 100)]
        [Display(Name = "Liczba serii")]
        public int Sets { get; set; }

        [Range(1, 100)]
        [Display(Name = "Liczba powtórzeń w serii")]
        public int Repetitions { get; set; }
    }
}

