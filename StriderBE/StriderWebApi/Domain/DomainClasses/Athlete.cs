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
        public string? VO2Max { get; set; } // Velocidad máxima por km en formato mm:ss (ejemplo: "03:30")
        public int YearsOfExperience { get; set; } = 0; // Default to 0 if not provided
        public DateTime? TrainingStartDate { get; set; } // Fecha aproximada de inicio de entrenamiento (mes y año, día 1)
        public TrainingVolumeType TrainingVolumeType { get; set; } = TrainingVolumeType.Weekly; // Default to Weekly if not provided
        public int TrainingVolumeKm { get; set; } = 0; // Default to 0 if not provided
        public List<string> MedicalConditions { get; set; } = [];
        public List<string> Objectives { get; set; } = [];
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public virtual List<AthleteInjury> Injuries { get; set; } = [];
        public virtual List<Workout> Workouts { get; set; } = [];
    }
}
