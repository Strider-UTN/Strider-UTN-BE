using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Distance { get; set; }
        public DateTime Date { get; set; }
        public double Duration { get; set; }
        public double AverageHR { get; set; }
        public WorkoutState State { get; set; }
        public WorkoutType Type { get; set; }
        public string? Comments { get; set; }
        public string? CoachFeedback { get; set; }
        public bool IsReviewed { get; set; }
        
        // Navigation properties
        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; } = null!;
        
        public int? SessionId { get; set; }
        public Session? Session { get; set; }
        
        // Collections
        public List<Lap> Laps { get; set; } = [];
    }
} 