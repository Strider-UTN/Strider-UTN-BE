using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public class AthleteInjury
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public InjuryLocation? AffectedArea { get; set; }

        public InjurySeverity Severity { get; set; } = InjurySeverity.Mild;
        public InjuryStatus Status { get; set; } = InjuryStatus.Active;

        public DateTime DiagnosisDate { get; set; }
        public DateTime? RecoveryEstimateDate { get; set; }
        public DateTime? RecoveryDate { get; set; }

        public InjuryTreatmentType? Treatment { get; set; }
        public InjuryImpactLevel? ImpactOnTraining { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
