using System;
using System.ComponentModel.DataAnnotations;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Injuries
{
    public class UpdateAthleteInjuryDto
    {
        [Required]
        public InjuryStatus Status { get; set; }

        public InjurySeverity? Severity { get; set; }

        public DateTime? RecoveryEstimateDate { get; set; }

        public DateTime? RecoveryDate { get; set; }

        public InjuryTreatmentType? Treatment { get; set; }

        public InjuryImpactLevel? ImpactOnTraining { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}
