using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class TrainingEntry
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sesja treningowa", Description = "Wybierz sesję treningową")]
        public int TrainingSessionId { get; set; }
        public TrainingSession? TrainingSession { get; set; }

        [Required]
        [Display(Name = "Typ ćwiczenia", Description = "Wybierz typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }
        public ExerciseType? ExerciseType { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

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
