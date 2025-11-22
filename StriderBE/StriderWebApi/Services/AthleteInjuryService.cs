using System;
using System.Collections.Generic;
using System.Linq;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Injuries;
using StriderWebApi.Services.Interfaces;

namespace StriderWebApi.Services
{
    public class AthleteInjuryService(
        IAthleteRepository athleteRepository,
        IAthleteInjuryRepository athleteInjuryRepository,
        ICoachAthleteRelationshipRepository coachAthleteRelationshipRepository,
        INotificationService notificationService) : IAthleteInjuryService
    {
        private readonly IAthleteRepository _athleteRepository = athleteRepository;
        private readonly IAthleteInjuryRepository _athleteInjuryRepository = athleteInjuryRepository;
        private readonly ICoachAthleteRelationshipRepository _coachAthleteRelationshipRepository = coachAthleteRelationshipRepository;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<AthleteInjurySummaryDto> CreateAsync(int athleteId, CreateAthleteInjuryDto dto, CancellationToken cancellationToken = default)
        {
            var athlete = await _athleteRepository.GetAthleteByIdAsync(athleteId)
                ?? throw new KeyNotFoundException("Atleta no encontrado");

            if (dto.RecoveryDate.HasValue && dto.RecoveryDate.Value < dto.DiagnosisDate)
            {
                throw new ArgumentException("La fecha de recuperación no puede ser anterior a la fecha de diagnóstico");
            }

            if (dto.RecoveryEstimateDate.HasValue && dto.RecoveryEstimateDate.Value < dto.DiagnosisDate)
            {
                throw new ArgumentException("La fecha estimada de recuperación no puede ser anterior a la fecha de diagnóstico");
            }

            var injury = new AthleteInjury
            {
                AthleteId = athlete.Id,
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                AffectedArea = dto.AffectedArea,
                Severity = dto.Severity,
                Status = dto.Status,
                DiagnosisDate = dto.DiagnosisDate,
                RecoveryEstimateDate = dto.RecoveryEstimateDate,
                RecoveryDate = dto.RecoveryDate,
                Treatment = dto.Treatment,
                ImpactOnTraining = dto.ImpactOnTraining,
                Notes = dto.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _athleteInjuryRepository.CreateAsync(injury, cancellationToken);
            await NotifyCoachesOfNewInjuryAsync(athlete, created, cancellationToken);
            return MapToSummary(created);
        }

        public async Task<AthleteInjurySummaryDto?> GetByIdAsync(int injuryId, int athleteId, CancellationToken cancellationToken = default)
        {
            var injury = await _athleteInjuryRepository.GetByIdAsync(injuryId, cancellationToken);
            if (injury == null || injury.AthleteId != athleteId)
            {
                return null;
            }

            return MapToSummary(injury);
        }

        public async Task<IReadOnlyCollection<AthleteInjurySummaryDto>> GetByAthleteAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            var injuries = await _athleteInjuryRepository.GetByAthleteIdAsync(athleteId, cancellationToken);
            return injuries
                .OrderByDescending(i => i.DiagnosisDate)
                .ThenByDescending(i => i.Id)
                .Select(MapToSummary)
                .ToList();
        }

        public async Task<AthleteInjurySummaryDto> UpdateAsync(int athleteId, int injuryId, UpdateAthleteInjuryDto dto, CancellationToken cancellationToken = default)
        {
            var injury = await _athleteInjuryRepository.GetByIdAsync(injuryId, cancellationToken)
                ?? throw new KeyNotFoundException("Lesión no encontrada");

            if (injury.AthleteId != athleteId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para modificar esta lesión");
            }

            if (dto.RecoveryDate.HasValue && dto.RecoveryDate.Value < injury.DiagnosisDate)
            {
                throw new ArgumentException("La fecha de recuperación no puede ser anterior a la fecha de diagnóstico");
            }

            if (dto.RecoveryEstimateDate.HasValue && dto.RecoveryEstimateDate.Value < injury.DiagnosisDate)
            {
                throw new ArgumentException("La fecha estimada de recuperación no puede ser anterior a la fecha de diagnóstico");
            }

            injury.Status = dto.Status;
            if (dto.Severity.HasValue)
            {
                injury.Severity = dto.Severity.Value;
            }

            injury.RecoveryEstimateDate = dto.RecoveryEstimateDate;
            injury.RecoveryDate = dto.RecoveryDate;
            if (dto.Treatment.HasValue)
            {
                injury.Treatment = dto.Treatment;
            }

            if (dto.ImpactOnTraining.HasValue)
            {
                injury.ImpactOnTraining = dto.ImpactOnTraining;
            }

            injury.Notes = dto.Notes?.Trim();
            injury.UpdatedAt = DateTime.UtcNow;

            var updated = await _athleteInjuryRepository.UpdateAsync(injury, cancellationToken);
            return MapToSummary(updated);
        }

        public async Task<IReadOnlyCollection<CoachRecentInjuryDto>> GetRecentInjuriesForCoachAsync(int coachId, CancellationToken cancellationToken = default)
        {
            var injuries = await _athleteInjuryRepository.GetRecentInjuriesForCoachAsync(coachId, cancellationToken);

            return injuries
                .Select(injury => new CoachRecentInjuryDto
                {
                    InjuryId = injury.Id,
                    AthleteId = injury.AthleteId,
                    AthleteName = injury.Athlete?.FullName ?? "Atleta",
                    Title = injury.Title,
                    Severity = injury.Severity,
                    Status = injury.Status,
                    DiagnosisDate = injury.DiagnosisDate,
                    RecoveryEstimateDate = injury.RecoveryEstimateDate,
                    RecoveryDate = injury.RecoveryDate,
                    CreatedAt = injury.CreatedAt,
                    Treatment = injury.Treatment,
                    ImpactOnTraining = injury.ImpactOnTraining
                })
                .ToList();
        }

        public async Task<IReadOnlyCollection<CoachRecentInjuryDto>> GetTop3RecentInjuriesForAthleteAsync(int athleteId, CancellationToken cancellationToken = default)
        {
            var injuries = await _athleteInjuryRepository.GetTop3RecentInjuriesForAthleteAsync(athleteId, cancellationToken);

            return injuries
                .Select(injury => new CoachRecentInjuryDto
                {
                    InjuryId = injury.Id,
                    AthleteId = injury.AthleteId,
                    AthleteName = injury.Athlete?.FullName ?? "Atleta",
                    Title = injury.Title,
                    Severity = injury.Severity,
                    Status = injury.Status,
                    DiagnosisDate = injury.DiagnosisDate,
                    RecoveryEstimateDate = injury.RecoveryEstimateDate,
                    RecoveryDate = injury.RecoveryDate,
                    CreatedAt = injury.CreatedAt,
                    Treatment = injury.Treatment,
                    ImpactOnTraining = injury.ImpactOnTraining
                })
                .ToList();
        }

        private static AthleteInjurySummaryDto MapToSummary(AthleteInjury injury)
        {
            var treatment = injury.Treatment;
            var impact = injury.ImpactOnTraining;

            if (!treatment.HasValue || !impact.HasValue)
            {
                var parsed = ParseTreatmentAndImpact(injury.Notes);
                treatment ??= parsed.Treatment;
                impact ??= parsed.Impact;
            }

            return new AthleteInjurySummaryDto
            {
                Id = injury.Id,
                Title = injury.Title,
                Description = injury.Description,
                Severity = injury.Severity,
                Status = injury.Status,
                AffectedArea = injury.AffectedArea,
                DiagnosisDate = injury.DiagnosisDate,
                RecoveryEstimateDate = injury.RecoveryEstimateDate,
                RecoveryDate = injury.RecoveryDate,
                Treatment = treatment,
                ImpactOnTraining = impact,
                Notes = injury.Notes
            };
        }

        private static (InjuryTreatmentType? Treatment, InjuryImpactLevel? Impact) ParseTreatmentAndImpact(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
            {
                return (null, null);
            }

            InjuryTreatmentType? treatment = null;
            InjuryImpactLevel? impact = null;

            var lines = notes.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var line in lines)
            {
                if (treatment == null && line.StartsWith("Tratamiento", StringComparison.OrdinalIgnoreCase))
                {
                    treatment = MapTreatment(ExtractValue(line));
                }
                else if (impact == null && line.StartsWith("Impacto en entrenamiento", StringComparison.OrdinalIgnoreCase))
                {
                    impact = MapImpact(ExtractValue(line));
                }
            }

            return (treatment, impact);
        }

        private static string? ExtractValue(string line)
        {
            var separatorIndex = line.IndexOf(':');
            if (separatorIndex < 0 || separatorIndex + 1 >= line.Length)
            {
                return line.Trim();
            }

            return line[(separatorIndex + 1)..].Trim();
        }

        private static InjuryTreatmentType? MapTreatment(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();

            if (Enum.TryParse<InjuryTreatmentType>(value, true, out var parsedEnum))
            {
                return parsedEnum;
            }

            switch (value.ToLowerInvariant())
            {
                case "reposo":
                    return InjuryTreatmentType.Rest;
                case "fisioterapia":
                    return InjuryTreatmentType.Physiotherapy;
                case "medicación":
                case "medicacion":
                    return InjuryTreatmentType.Medication;
                case "rehabilitación":
                case "rehabilitacion":
                    return InjuryTreatmentType.Rehabilitation;
                case "terapia manual":
                    return InjuryTreatmentType.ManualTherapy;
                case "ejercicios específicos":
                case "ejercicios especificos":
                    return InjuryTreatmentType.SpecificExercises;
                case "crioterapia":
                    return InjuryTreatmentType.Cryotherapy;
                case "termoterapia":
                    return InjuryTreatmentType.Thermotherapy;
                case "electroterapia":
                    return InjuryTreatmentType.Electrotherapy;
                case "cirugía":
                case "cirugia":
                    return InjuryTreatmentType.Surgery;
                case "otro":
                    return InjuryTreatmentType.Other;
                default:
                    return null;
            }
        }

        private static InjuryImpactLevel? MapImpact(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();

            if (Enum.TryParse<InjuryImpactLevel>(value, true, out var parsedEnum))
            {
                return parsedEnum;
            }

            switch (value.ToLowerInvariant())
            {
                case "ninguno":
                    return InjuryImpactLevel.None;
                case "bajo":
                    return InjuryImpactLevel.Low;
                case "moderado":
                    return InjuryImpactLevel.Moderate;
                case "alto":
                    return InjuryImpactLevel.High;
                case "completo":
                case "completo (no puedo entrenar)":
                    return InjuryImpactLevel.Full;
                default:
                    return null;
            }
        }

        private async Task NotifyCoachesOfNewInjuryAsync(Athlete athlete, AthleteInjury injury, CancellationToken cancellationToken)
        {
            var relationships = await _coachAthleteRelationshipRepository
                .GetRelationshipsByAthleteAsync(athlete.Id, CoachAthleteRelationshipStatus.Accepted, cancellationToken);

            if (relationships.Count == 0)
            {
                return;
            }

            var notifications = relationships.Select(rel => new Notification
            {
                UserId = rel.CoachId,
                Title = "Nueva lesión registrada",
                Message = $"{athlete.FullName} registró la lesión '{injury.Title}' el {injury.DiagnosisDate:dd/MM/yyyy}.",
                Type = NotificationType.Injury,
                CreatedBy = athlete.FullName,
                LinkUrl = $"/coach/athletes/{athlete.Id}"
            }).ToList();

            await _notificationService.CreateRangeAsync(notifications, cancellationToken);
        }
    }
}
