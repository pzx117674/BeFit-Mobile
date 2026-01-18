using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class TrainingSessionCreateDto
    {
        [Display(Name = "Start treningu")]
        public DateTime StartedAt { get; set; }

        [Display(Name = "Koniec treningu")]
        public DateTime EndedAt { get; set; }
    }
}

