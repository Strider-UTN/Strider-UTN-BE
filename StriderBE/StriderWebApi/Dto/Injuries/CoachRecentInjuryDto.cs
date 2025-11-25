using System;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Injuries
{
    public class CoachRecentInjuryDto
    {
        public int InjuryId { get; set; }
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public InjurySeverity Severity { get; set; }
        public InjuryStatus Status { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public DateTime? RecoveryEstimateDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public InjuryTreatmentType? Treatment { get; set; }
        public InjuryImpactLevel? ImpactOnTraining { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public InjuryLocation? AffectedArea { get; set; }
    }
}
