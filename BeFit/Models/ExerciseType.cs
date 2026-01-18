using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        [Display(Name = "Nazwa ćwiczenia", Description = "Nazwa typu ćwiczenia")]
        public string Name { get; set; } = string.Empty;
    }
}
