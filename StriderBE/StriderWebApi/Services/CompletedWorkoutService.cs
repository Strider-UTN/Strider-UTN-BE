using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.CompletedWorkout;
using StriderWebApi.Exceptions;
using StriderWebApi.Services.Interfaces;
using System.Globalization;

namespace StriderWebApi.Services
{
    public class CompletedWorkoutService(
        ICompletedWorkoutRepository completedWorkoutRepository,
        ITrainingSessionAthleteRepository trainingSessionAthleteRepository,
        StriderDbContext context) : ICompletedWorkoutService
    {
        public async Task<CompletedWorkoutResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var workout = await completedWorkoutRepository.GetByIdAsync(id, cancellationToken);
            if (workout == null)
            {
                throw new NotFoundException("Entrenamiento completado no encontrado");
            }

            return MapToResponseDto(workout);
        }

        public async Task<CompletedWorkoutResponseDto> CreateAsync(CreateCompletedWorkoutDto dto, int athleteId, CancellationToken cancellationToken = default)
        {
            // Validar que el TrainingSessionAthlete pertenezca al atleta autenticado
            var trainingSessionAthlete = await trainingSessionAthleteRepository.GetByIdAsync(dto.TrainingSessionAthleteId, cancellationToken);
            if (trainingSessionAthlete == null)
            {
                throw new NotFoundException("Sesión de entrenamiento no encontrada");
            }

            if (trainingSessionAthlete.AthleteId != athleteId)
            {
                throw new UnauthorizedException("No tienes permisos para crear un entrenamiento para esta sesión");
            }

            // Convertir duración de mm:ss a segundos
            var durationSeconds = ParseDurationToSeconds(dto.Duration);
            if (durationSeconds <= 0)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("La duración debe estar en formato mm:ss y ser mayor a 0");
            }

            // Validar sensaciones si se proporcionan
            if (dto.Sensations != null)
            {
                ValidateSensations(dto.Sensations);
            }

            // Crear la entidad CompletedWorkout
            var completedWorkout = new CompletedWorkout
            {
                TrainingSessionAthleteId = dto.TrainingSessionAthleteId,
                Name = dto.Name,
                Distance = dto.Distance,
                Date = dto.Date.Kind == DateTimeKind.Utc ? dto.Date : DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                Duration = durationSeconds,
                AverageHR = dto.AverageHR,
                Comments = dto.Comments,
                Source = dto.Source
            };

            // Agregar sensaciones si se proporcionan
            if (dto.Sensations != null)
            {
                completedWorkout.Sensations = new WorkoutSensations
                {
                    Effort = dto.Sensations.Effort,
                    Fatigue = dto.Sensations.Fatigue,
                    Motivation = dto.Sensations.Motivation,
                    MuscularLoad = dto.Sensations.MuscularLoad,
                    OverallFeeling = dto.Sensations.OverallFeeling
                };
            }

            // Agregar laps si se proporcionan
            if (dto.Laps != null && dto.Laps.Count > 0)
            {
                completedWorkout.Laps = dto.Laps.Select(lapDto => new WorkoutLap
                {
                    Index = lapDto.Index,
                    Distance = lapDto.Distance,
                    Duration = lapDto.Duration,
                    AverageHR = lapDto.AverageHR,
                    Speed = lapDto.Distance > 0 && lapDto.Duration > 0 
                        ? (lapDto.Distance * 1000) / lapDto.Duration // m/s
                        : 0,
                    StartTime = lapDto.StartTime.Kind == DateTimeKind.Utc 
                        ? lapDto.StartTime 
                        : DateTime.SpecifyKind(lapDto.StartTime, DateTimeKind.Utc)
                }).ToList();
            }

            // Agregar injuries si se proporcionan
            if (dto.Injuries != null && dto.Injuries.Count > 0)
            {
                completedWorkout.Injuries = dto.Injuries.Select(injuryDto => new WorkoutInjury
                {
                    BodyPart = injuryDto.BodyPart,
                    Severity = injuryDto.Severity,
                    Description = injuryDto.Description,
                    AffectedPerformance = injuryDto.AffectedPerformance,
                    Type = injuryDto.Type
                }).ToList();
            }

            var createdWorkout = await completedWorkoutRepository.CreateAsync(completedWorkout, cancellationToken);
            
            // Recargar con todas las relaciones
            var workoutWithRelations = await completedWorkoutRepository.GetByIdAsync(createdWorkout.Id, cancellationToken);
            return MapToResponseDto(workoutWithRelations ?? createdWorkout);
        }

        public async Task<CompletedWorkoutResponseDto> UpdateAsync(int id, CreateCompletedWorkoutDto dto, int athleteId, CancellationToken cancellationToken = default)
        {
            var existingWorkout = await completedWorkoutRepository.GetByIdAsync(id, cancellationToken);
            if (existingWorkout == null)
            {
                throw new NotFoundException("Entrenamiento completado no encontrado");
            }

            // Validar acceso
            if (!await ValidateWorkoutAccessAsync(id, athleteId, cancellationToken))
            {
                throw new UnauthorizedException("No tienes permisos para actualizar este entrenamiento");
            }

            // Validar que el TrainingSessionAthlete pertenezca al atleta
            var trainingSessionAthlete = await trainingSessionAthleteRepository.GetByIdAsync(dto.TrainingSessionAthleteId, cancellationToken);
            if (trainingSessionAthlete == null || trainingSessionAthlete.AthleteId != athleteId)
            {
                throw new UnauthorizedException("No tienes permisos para actualizar este entrenamiento");
            }

            // Convertir duración
            var durationSeconds = ParseDurationToSeconds(dto.Duration);
            if (durationSeconds <= 0)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("La duración debe estar en formato mm:ss y ser mayor a 0");
            }

            // Validar sensaciones si se proporcionan
            if (dto.Sensations != null)
            {
                ValidateSensations(dto.Sensations);
            }

            // Actualizar campos básicos
            existingWorkout.Name = dto.Name;
            existingWorkout.Distance = dto.Distance;
            existingWorkout.Date = dto.Date.Kind == DateTimeKind.Utc ? dto.Date : DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
            existingWorkout.Duration = durationSeconds;
            existingWorkout.AverageHR = dto.AverageHR;
            existingWorkout.Comments = dto.Comments;
            existingWorkout.Source = dto.Source;

            // Actualizar sensaciones
            if (dto.Sensations != null)
            {
                if (existingWorkout.Sensations == null)
                {
                    existingWorkout.Sensations = new WorkoutSensations();
                }
                existingWorkout.Sensations.Effort = dto.Sensations.Effort;
                existingWorkout.Sensations.Fatigue = dto.Sensations.Fatigue;
                existingWorkout.Sensations.Motivation = dto.Sensations.Motivation;
                existingWorkout.Sensations.MuscularLoad = dto.Sensations.MuscularLoad;
                existingWorkout.Sensations.OverallFeeling = dto.Sensations.OverallFeeling;
            }

            // Actualizar laps (eliminar los existentes y crear nuevos)
            if (dto.Laps != null)
            {
                // Eliminar laps existentes
                foreach (var lap in existingWorkout.Laps.ToList())
                {
                    // EF Core manejará la eliminación en cascada, pero podemos hacerlo explícitamente
                }
                existingWorkout.Laps.Clear();

                // Agregar nuevos laps
                existingWorkout.Laps = dto.Laps.Select(lapDto => new WorkoutLap
                {
                    Index = lapDto.Index,
                    Distance = lapDto.Distance,
                    Duration = lapDto.Duration,
                    AverageHR = lapDto.AverageHR,
                    Speed = lapDto.Distance > 0 && lapDto.Duration > 0 
                        ? (lapDto.Distance * 1000) / lapDto.Duration
                        : 0,
                    StartTime = lapDto.StartTime.Kind == DateTimeKind.Utc 
                        ? lapDto.StartTime 
                        : DateTime.SpecifyKind(lapDto.StartTime, DateTimeKind.Utc)
                }).ToList();
            }

            // Actualizar injuries (eliminar los existentes y crear nuevos)
            if (dto.Injuries != null)
            {
                existingWorkout.Injuries.Clear();
                existingWorkout.Injuries = dto.Injuries.Select(injuryDto => new WorkoutInjury
                {
                    BodyPart = injuryDto.BodyPart,
                    Severity = injuryDto.Severity,
                    Description = injuryDto.Description,
                    AffectedPerformance = injuryDto.AffectedPerformance,
                    Type = injuryDto.Type
                }).ToList();
            }

            var updatedWorkout = await completedWorkoutRepository.UpdateAsync(existingWorkout, cancellationToken);
            
            // Recargar con todas las relaciones
            var workoutWithRelations = await completedWorkoutRepository.GetByIdAsync(updatedWorkout.Id, cancellationToken);
            return MapToResponseDto(workoutWithRelations ?? updatedWorkout);
        }

        public async Task<bool> DeleteAsync(int id, int athleteId, CancellationToken cancellationToken = default)
        {
            if (!await ValidateWorkoutAccessAsync(id, athleteId, cancellationToken))
            {
                throw new UnauthorizedException("No tienes permisos para eliminar este entrenamiento");
            }

            return await completedWorkoutRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<CompletedWorkoutResponseDto>> GetByTrainingSessionAthleteIdAsync(int trainingSessionAthleteId, CancellationToken cancellationToken = default)
        {
            var workouts = await completedWorkoutRepository.GetByTrainingSessionAthleteIdAsync(trainingSessionAthleteId, cancellationToken);
            return workouts.Select(MapToResponseDto);
        }

        public async Task<CompletedWorkoutResponseDto?> GetByTrainingSessionAthleteIdAndDateAsync(int trainingSessionAthleteId, DateTime date, CancellationToken cancellationToken = default)
        {
            var workout = await completedWorkoutRepository.GetByTrainingSessionAthleteIdAndDateAsync(trainingSessionAthleteId, date, cancellationToken);
            return workout != null ? MapToResponseDto(workout) : null;
        }

        public async Task<IEnumerable<CompletedWorkoutResponseDto>> GetMyCompletedWorkoutsAsync(int athleteId, DateTime? date = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<CompletedWorkout> workouts;
            
            if (date.HasValue)
            {
                workouts = await completedWorkoutRepository.GetByAthleteIdAndDateAsync(athleteId, date.Value, cancellationToken);
            }
            else
            {
                workouts = await completedWorkoutRepository.GetByAthleteIdAsync(athleteId, cancellationToken);
            }

            return workouts.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<CompletedWorkoutResponseDto>> GetMyCompletedWorkoutsByDateRangeAsync(int athleteId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var workouts = await completedWorkoutRepository.GetByAthleteIdAndDateRangeAsync(athleteId, startDate, endDate, cancellationToken);
            return workouts.Select(MapToResponseDto);
        }

        public async Task<bool> ValidateWorkoutAccessAsync(int workoutId, int athleteId, CancellationToken cancellationToken = default)
        {
            var workout = await completedWorkoutRepository.GetByIdAsync(workoutId, cancellationToken);
            if (workout == null)
            {
                return false;
            }

            return workout.TrainingSessionAthlete.AthleteId == athleteId;
        }

        public async Task<IEnumerable<CompletedWorkoutsGroupedByAthleteDto>> GetForCoachWithFiltersGroupedByAthleteAsync(
            int coachId,
            int? planningId = null,
            int? trainingGroupId = null,
            int? athleteId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? hasFeedback = null,
            CancellationToken cancellationToken = default)
        {
            var workouts = await completedWorkoutRepository.GetForCoachWithFiltersAsync(
                coachId, planningId, trainingGroupId, athleteId, startDate, endDate, hasFeedback, cancellationToken);

            // Obtener los grupos de entrenamiento para los atletas
            var athleteIds = workouts.Select(w => w.TrainingSessionAthlete.AthleteId).Distinct().ToList();
            var groupMembers = await context.TrainingGroupMembers
                .Include(tgm => tgm.TrainingGroup)
                .Where(tgm => athleteIds.Contains(tgm.UserId) && tgm.Status == Domain.Enums.TrainingGroupMemberStatus.Active)
                .ToListAsync(cancellationToken);

            var groupMembersByAthlete = groupMembers
                .GroupBy(tgm => tgm.UserId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            // Agrupar por atleta
            var grouped = workouts
                .GroupBy(w => new { w.TrainingSessionAthlete.AthleteId, w.TrainingSessionAthlete.Athlete })
                .Select(g =>
                {
                    var athleteGroupMember = groupMembersByAthlete.GetValueOrDefault(g.Key.AthleteId);
                    return new CompletedWorkoutsGroupedByAthleteDto
                    {
                        AthleteId = g.Key.AthleteId,
                        AthleteName = g.Key.Athlete.FullName ?? g.Key.Athlete.Username,
                        TrainingGroupId = athleteGroupMember?.TrainingGroupId,
                        TrainingGroupName = athleteGroupMember?.TrainingGroup?.Name,
                        Workouts = g.Select(MapToResponseDto).OrderByDescending(w => w.Date).ThenByDescending(w => w.CreatedAt).ToList()
                    };
                })
                .OrderBy(g => g.AthleteName)
                .ToList();

            return grouped;
        }

        // Métodos auxiliares
        private static int ParseDurationToSeconds(string durationStr)
        {
            if (string.IsNullOrWhiteSpace(durationStr) || !durationStr.Contains(':'))
            {
                return 0;
            }

            var parts = durationStr.Split(':');
            if (parts.Length != 2)
            {
                return 0;
            }

            if (!int.TryParse(parts[0], out var minutes) || !int.TryParse(parts[1], out var seconds))
            {
                return 0;
            }

            return (minutes * 60) + seconds;
        }

        private static void ValidateSensations(CreateWorkoutSensationsDto sensations)
        {
            if (sensations.Effort < 1 || sensations.Effort > 10 ||
                sensations.Fatigue < 1 || sensations.Fatigue > 10 ||
                sensations.Motivation < 1 || sensations.Motivation > 10 ||
                sensations.MuscularLoad < 1 || sensations.MuscularLoad > 10 ||
                sensations.OverallFeeling < 1 || sensations.OverallFeeling > 10)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("Las sensaciones deben estar en el rango de 1 a 10");
            }
        }

        private static CompletedWorkoutResponseDto MapToResponseDto(CompletedWorkout workout)
        {
            return new CompletedWorkoutResponseDto
            {
                Id = workout.Id,
                Name = workout.Name,
                Distance = workout.Distance,
                Date = workout.Date,
                Duration = workout.Duration,
                AverageHR = workout.AverageHR,
                Comments = workout.Comments,
                Source = workout.Source,
                TrainingSessionAthleteId = workout.TrainingSessionAthleteId,
                TrainingSessionId = workout.TrainingSessionAthlete.TrainingSession.Id,
                TrainingSessionName = workout.TrainingSessionAthlete.TrainingSession.Name,
                AthleteId = workout.TrainingSessionAthlete.AthleteId,
                AthleteName = workout.TrainingSessionAthlete.Athlete.FullName ?? workout.TrainingSessionAthlete.Athlete.Username,
                PlanningId = workout.TrainingSessionAthlete.TrainingSession.Planning?.Id,
                PlanningName = workout.TrainingSessionAthlete.TrainingSession.Planning?.Name,
                MesocycleId = workout.TrainingSessionAthlete.TrainingSession.Microcycle?.MesocycleId,
                MesocycleName = workout.TrainingSessionAthlete.TrainingSession.Microcycle?.Mesocycle?.Name,
                MicrocycleId = workout.TrainingSessionAthlete.TrainingSession.MicrocycleId,
                MicrocycleName = workout.TrainingSessionAthlete.TrainingSession.Microcycle?.Name,
                Sensations = workout.Sensations != null ? new WorkoutSensationsResponseDto
                {
                    Id = workout.Sensations.Id,
                    Effort = workout.Sensations.Effort,
                    Fatigue = workout.Sensations.Fatigue,
                    Motivation = workout.Sensations.Motivation,
                    MuscularLoad = workout.Sensations.MuscularLoad,
                    OverallFeeling = workout.Sensations.OverallFeeling
                } : null,
                Laps = workout.Laps.OrderBy(l => l.Index).Select(lap => new WorkoutLapResponseDto
                {
                    Id = lap.Id,
                    Index = lap.Index,
                    Distance = lap.Distance,
                    Duration = lap.Duration,
                    AverageHR = lap.AverageHR,
                    Speed = lap.Speed,
                    StartTime = lap.StartTime
                }).ToList(),
                Injuries = workout.Injuries.Select(injury => new WorkoutInjuryResponseDto
                {
                    Id = injury.Id,
                    BodyPart = injury.BodyPart,
                    Severity = injury.Severity,
                    Description = injury.Description,
                    AffectedPerformance = injury.AffectedPerformance,
                    Type = injury.Type
                }).ToList(),
                Feedback = workout.Feedback != null ? new WorkoutFeedbackResponseDto
                {
                    Id = workout.Feedback.Id,
                    CoachId = workout.Feedback.CoachId,
                    CoachName = workout.Feedback.Coach != null 
                        ? (workout.Feedback.Coach.FullName ?? workout.Feedback.Coach.Username)
                        : "Entrenador",
                    Feedback = workout.Feedback.Feedback,
                    Recommendations = workout.Feedback.Recommendations,
                    Rating = workout.Rating.HasValue 
                        ? workout.Rating.Value 
                        : WorkoutRating.DoesNotMeetObjectives,
                    LapFeedbacks = workout.Feedback.LapFeedbacks?.Select(lf => new LapFeedbackResponseDto
                    {
                        Id = lf.Id,
                        WorkoutLapId = lf.WorkoutLapId,
                        Feedback = lf.Feedback,
                        CreatedAt = lf.CreatedAt
                    }).ToList() ?? new List<LapFeedbackResponseDto>(),
                    CreatedAt = workout.Feedback.CreatedAt,
                    UpdatedAt = workout.Feedback.UpdatedAt
                } : null,
                Rating = workout.Rating,
                CreatedAt = workout.CreatedAt,
                UpdatedAt = workout.UpdatedAt
            };
        }

        public async Task<WorkoutFeedbackResponseDto> SubmitWorkoutFeedbackAsync(int completedWorkoutId, CreateWorkoutFeedbackDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var workout = await completedWorkoutRepository.GetByIdAsync(completedWorkoutId, cancellationToken);
            if (workout == null)
            {
                throw new NotFoundException("Entrenamiento completado no encontrado");
            }

            // Validar que el coach tenga acceso a este workout (a través de la planificación)
            var planning = workout.TrainingSessionAthlete.TrainingSession.Planning;
            if (planning == null || planning.CoachId != coachId)
            {
                throw new UnauthorizedException("No tienes permisos para dar feedback a este entrenamiento");
            }

            // Verificar si ya existe feedback
            if (workout.Feedback != null)
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("Ya existe feedback para este entrenamiento. Usa el endpoint de actualización.");
            }

            // Crear nuevo feedback
            var feedback = new WorkoutFeedback
            {
                CompletedWorkoutId = completedWorkoutId,
                CoachId = coachId,
                Feedback = dto.Feedback,
                Recommendations = dto.Recommendations,
                CreatedAt = DateTime.UtcNow
            };

            // Crear feedbacks de laps si existen
            if (dto.LapFeedbacks != null && dto.LapFeedbacks.Count > 0)
            {
                foreach (var lapFeedbackDto in dto.LapFeedbacks)
                {
                    // Validar que el lap existe y pertenece al workout
                    var lap = workout.Laps.FirstOrDefault(l => l.Id == lapFeedbackDto.WorkoutLapId);
                    if (lap == null)
                    {
                        throw new NotFoundException($"Lap con ID {lapFeedbackDto.WorkoutLapId} no encontrado");
                    }

                    feedback.LapFeedbacks.Add(new LapFeedback
                    {
                        WorkoutLapId = lapFeedbackDto.WorkoutLapId,
                        Feedback = lapFeedbackDto.Feedback,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            context.WorkoutFeedbacks.Add(feedback);
            
            // Actualizar el rating en el CompletedWorkout
            workout.Rating = dto.Rating;
            
            await context.SaveChangesAsync(cancellationToken);

            // Recargar el workout con el feedback
            workout = await completedWorkoutRepository.GetByIdAsync(completedWorkoutId, cancellationToken);
            if (workout == null || workout.Feedback == null)
            {
                throw new NotFoundException("Error al recargar el entrenamiento después de crear el feedback");
            }

            return MapToFeedbackResponseDto(workout.Feedback, workout.Rating);
        }

        public async Task<WorkoutFeedbackResponseDto> UpdateWorkoutFeedbackAsync(int completedWorkoutId, UpdateWorkoutFeedbackDto dto, int coachId, CancellationToken cancellationToken = default)
        {
            var workout = await completedWorkoutRepository.GetByIdAsync(completedWorkoutId, cancellationToken);
            if (workout == null)
            {
                throw new NotFoundException("Entrenamiento completado no encontrado");
            }

            // Validar que el coach tenga acceso a este workout
            var planning = workout.TrainingSessionAthlete.TrainingSession.Planning;
            if (planning == null || planning.CoachId != coachId)
            {
                throw new UnauthorizedException("No tienes permisos para actualizar el feedback de este entrenamiento");
            }

            // Verificar si existe feedback
            if (workout.Feedback == null)
            {
                throw new NotFoundException("No existe feedback para este entrenamiento. Usa el endpoint de creación.");
            }

            // Validar que el feedback pertenece al coach
            if (workout.Feedback.CoachId != coachId)
            {
                throw new UnauthorizedException("No tienes permisos para actualizar este feedback");
            }

            var feedback = workout.Feedback;

            // Actualizar campos si se proporcionan
            if (dto.Rating.HasValue)
            {
                workout.Rating = dto.Rating.Value;
            }

            if (!string.IsNullOrEmpty(dto.Feedback))
            {
                feedback.Feedback = dto.Feedback;
            }

            if (dto.Recommendations != null)
            {
                feedback.Recommendations = dto.Recommendations;
            }

            feedback.UpdatedAt = DateTime.UtcNow;

            // Actualizar feedbacks de laps
            if (dto.LapFeedbacks != null)
            {
                // Eliminar feedbacks de laps existentes
                var existingLapFeedbacks = context.LapFeedbacks.Where(lf => lf.WorkoutFeedbackId == feedback.Id).ToList();
                context.LapFeedbacks.RemoveRange(existingLapFeedbacks);

                // Crear nuevos feedbacks de laps
                foreach (var lapFeedbackDto in dto.LapFeedbacks)
                {
                    // Validar que el lap existe y pertenece al workout
                    var lap = workout.Laps.FirstOrDefault(l => l.Id == lapFeedbackDto.WorkoutLapId);
                    if (lap == null)
                    {
                        throw new NotFoundException($"Lap con ID {lapFeedbackDto.WorkoutLapId} no encontrado");
                    }

                    feedback.LapFeedbacks.Add(new LapFeedback
                    {
                        WorkoutLapId = lapFeedbackDto.WorkoutLapId,
                        Feedback = lapFeedbackDto.Feedback,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            // Recargar el workout con el feedback actualizado
            workout = await completedWorkoutRepository.GetByIdAsync(completedWorkoutId, cancellationToken);
            if (workout == null || workout.Feedback == null)
            {
                throw new NotFoundException("Error al recargar el entrenamiento después de actualizar el feedback");
            }

            return MapToFeedbackResponseDto(workout.Feedback, workout.Rating);
        }

        private static WorkoutFeedbackResponseDto MapToFeedbackResponseDto(WorkoutFeedback feedback, WorkoutRating? rating = null)
        {
            return new WorkoutFeedbackResponseDto
            {
                Id = feedback.Id,
                CoachId = feedback.CoachId,
                CoachName = feedback.Coach != null 
                    ? (feedback.Coach.FullName ?? feedback.Coach.Username)
                    : "Entrenador",
                Feedback = feedback.Feedback,
                Recommendations = feedback.Recommendations,
                Rating = rating.HasValue 
                    ? rating.Value 
                    : WorkoutRating.DoesNotMeetObjectives,
                LapFeedbacks = feedback.LapFeedbacks?.Select(lf => new LapFeedbackResponseDto
                {
                    Id = lf.Id,
                    WorkoutLapId = lf.WorkoutLapId,
                    Feedback = lf.Feedback,
                    CreatedAt = lf.CreatedAt
                }).ToList() ?? new List<LapFeedbackResponseDto>(),
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = feedback.UpdatedAt
            };
        }
    }
}

