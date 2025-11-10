using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System;

namespace StriderWebApi.Dto.Injuries
{
    public class CreateAthleteInjuryDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public InjuryLocation? AffectedArea { get; set; }

        [Required]
        public InjurySeverity Severity { get; set; } = InjurySeverity.Moderate;

        public InjuryStatus Status { get; set; } = InjuryStatus.Active;

        [Required]
        public DateTime DiagnosisDate { get; set; }

        public DateTime? RecoveryEstimateDate { get; set; }

        public DateTime? RecoveryDate { get; set; }

        public InjuryTreatmentType? Treatment { get; set; }

        public InjuryImpactLevel? ImpactOnTraining { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}
