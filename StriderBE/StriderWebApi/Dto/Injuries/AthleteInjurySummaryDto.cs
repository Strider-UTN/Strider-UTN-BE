using System;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Injuries
{
    public class AthleteInjurySummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public InjurySeverity Severity { get; set; } = InjurySeverity.Mild;
        public InjuryStatus Status { get; set; } = InjuryStatus.Active;
        public InjuryLocation? AffectedArea { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime? RecoveryEstimateDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public InjuryTreatmentType? Treatment { get; set; }
        public InjuryImpactLevel? ImpactOnTraining { get; set; }
        public string? Notes { get; set; }
    }
}
