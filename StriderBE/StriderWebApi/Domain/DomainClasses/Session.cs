using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Session
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Warmup { get; set; } = string.Empty;
        public string CoolDown { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public WorkoutType SessionType { get; set; }
        
        // Navigation properties
        public int? TrainingPlanId { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        
        // Collections
        public List<Interval> Intervals { get; set; } = [];
        public List<Workout> Workouts { get; set; } = [];
    }
} 