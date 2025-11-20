using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class TrainingPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int CoachId { get; set; }
        public Coach Coach { get; set; } = null!;
        public List<TrainingSession> Sessions { get; set; } = [];
    }
} 