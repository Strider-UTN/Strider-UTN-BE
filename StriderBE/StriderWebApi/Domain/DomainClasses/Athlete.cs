using StriderWebApi.Domain.Enums;
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
        public int YearsOfExperience { get; set; } = 0; // Default to 0 if not provided
        public TrainingVolumeType TrainingVolumeType { get; set; } = TrainingVolumeType.Weekly; // Default to Weekly if not provided
        public int TrainingVolumeKm { get; set; } = 0; // Default to 0 if not provided
        public List<string> MedicalConditions { get; set; } = [];
        public List<string> Objectives { get; set; } = [];
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public virtual List<AthleteInjury> Injuries { get; set; } = [];
        public virtual List<Workout> Workouts { get; set; } = [];

        // TODO :: Should be getting these from SOMEWHERE else. Maybe Garmin?
        public double RestingHeartRate { get; set; } = 60;
        public double MaximumHeartRate { get; set; } = 200;
        public double ThresholdHeartRate { get; set; } = 150;
    }
}
