using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class TrainingSession : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Display(Name = "Start treningu")]
        public DateTime StartedAt { get; set; }

        [Display(Name = "Koniec treningu")]
        public DateTime EndedAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndedAt < StartedAt)
            {
                yield return new ValidationResult("Data zakończenia musi być późniejsza niż data rozpoczęcia.",
                    new[] { nameof(EndedAt) });
            }
        }
    }
}
