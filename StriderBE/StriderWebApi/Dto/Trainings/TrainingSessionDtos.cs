using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using static StriderWebApi.Dto.Trainings.TrainingTemplateDto;

namespace StriderWebApi.Dto.Trainings
{
    // CreateTrainingSessionDto.cs
    public class CreateTrainingSessionDto
    {
        [Required(ErrorMessage = "El nombre de la sesión es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public TrainingCategory Category { get; set; }

        [StringLength(2000, ErrorMessage = "Las notas no pueden exceder 2000 caracteres")]
        public string? Notes { get; set; }

        public int? TemplateId { get; set; } // Opcional: si fue creada desde una plantilla

        [Required(ErrorMessage = "Debe seleccionar al menos un atleta")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos un atleta")]
        public List<int> AthleteIds { get; set; } = new();

        public List<CreateTrainingIntervalDto> Intervals { get; set; } = new();
    }

    // TrainingSessionResponseDto.cs
    public class TrainingSessionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TrainingCategory Category { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int? TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TrainingSessionAthleteResponseDto> Athletes { get; set; } = new List<TrainingSessionAthleteResponseDto>();
        public List<TrainingIntervalResponseDto> Intervals { get; set; } = new List<TrainingIntervalResponseDto>();
    }

    // TrainingSessionAthleteResponseDto.cs
    public class TrainingSessionAthleteResponseDto
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Completed, Missed, Cancelled
        public DateTime? CompletedAt { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
