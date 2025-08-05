using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Athlete : User
    {
        // Domain-specific fields (keep existing)
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public string Country { get; set; } = string.Empty;
        
        // Model-matching fields
        public double? VO2Max { get; set; }
        public List<string> MedicalConditions { get; set; } = [];
        public List<string> Objectives { get; set; } = [];
        
        // Navigation properties to match model relationships
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        
        // Collections to match model
        public List<Ailment> Ailments { get; set; } = [];
        public List<Workout> Workouts { get; set; } = [];
    }
}
