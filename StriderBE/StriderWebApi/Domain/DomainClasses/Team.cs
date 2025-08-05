using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; }
        public bool AutomaticInscription { get; set; }
        public bool RequiresManualApproval { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        
        // Collections
        public List<TrainingLocation> TrainingLocations { get; set; } = [];
        public List<Athlete> Athletes { get; set; } = [];
        public List<Coach> Coaches { get; set; } = [];
    }
} 