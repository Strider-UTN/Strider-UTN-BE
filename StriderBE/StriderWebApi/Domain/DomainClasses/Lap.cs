using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Lap
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public double Speed { get; set; }
        public double HR { get; set; }
        public DateTime StartTime { get; set; }
        public string? CoachFeedback { get; set; }
        
        // Navigation property
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;
    }
} 