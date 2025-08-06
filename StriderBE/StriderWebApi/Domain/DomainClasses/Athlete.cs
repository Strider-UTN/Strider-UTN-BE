using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Athlete : User
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double? VO2Max { get; set; }
        public List<string> MedicalConditions { get; set; } = [];
        public List<string> Objectives { get; set; } = [];
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public List<Ailment> Ailments { get; set; } = [];
        public List<Workout> Workouts { get; set; } = [];
    }
}
