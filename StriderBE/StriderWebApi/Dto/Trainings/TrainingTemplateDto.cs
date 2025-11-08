using StriderWebApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Trainings
{
    /// <summary>
    /// DTO para crear una plantilla de entrenamiento
    /// </summary>
    public class CreateTrainingTemplateDto
    {
        [Required(ErrorMessage = "El nombre de la plantilla es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de entrenamiento es requerido")]
        public TrainingType Type { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public TrainingCategory Category { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor a 0")]
        public int Duration { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La distancia no puede ser negativa")]
        public decimal? Distance { get; set; }

        [StringLength(10, ErrorMessage = "El ritmo objetivo no puede exceder 10 caracteres")]
        public string? TargetPace { get; set; }

        [StringLength(50, ErrorMessage = "La frecuencia cardíaca objetivo no puede exceder 50 caracteres")]
        public string? TargetHR { get; set; }

        [StringLength(2000, ErrorMessage = "Las notas no pueden exceder 2000 caracteres")]
        public string Notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dificultad es requerida")]
        [Range(1, 5, ErrorMessage = "La dificultad debe estar entre 1 y 5")]
        public TrainingDifficulty Difficulty { get; set; }

        public string[] Tags { get; set; } = Array.Empty<string>();

        [Range(0, int.MaxValue, ErrorMessage = "La duración del calentamiento no puede ser negativa")]
        public int? WarmUpDuration { get; set; }

        [StringLength(10, ErrorMessage = "El ritmo del calentamiento no puede exceder 10 caracteres")]
        public string? WarmUpPace { get; set; }

        [StringLength(500, ErrorMessage = "La descripción del calentamiento no puede exceder 500 caracteres")]
        public string? WarmUpDescription { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La duración del enfriamiento no puede ser negativa")]
        public int? CoolDownDuration { get; set; }

        [StringLength(10, ErrorMessage = "El ritmo del enfriamiento no puede exceder 10 caracteres")]
        public string? CoolDownPace { get; set; }

        [StringLength(500, ErrorMessage = "La descripción del enfriamiento no puede exceder 500 caracteres")]
        public string? CoolDownDescription { get; set; }

        /// <summary>
        /// Nueva estructura basada en series. Preferir este campo.
        /// </summary>
        public List<CreateTrainingSeriesDto>? Series { get; set; }
    }

    /// <summary>
    /// DTO para crear una serie dentro de una plantilla
    /// </summary>
    public class CreateTrainingSeriesDto
    {
        [StringLength(200, ErrorMessage = "El nombre de la serie no puede exceder 200 caracteres")]
        public string Name { get; set; } = "Serie Principal";

        [Range(1, int.MaxValue, ErrorMessage = "Las repeticiones de la serie deben ser al menos 1")]
        public int Repetitions { get; set; } = 1;

        [StringLength(10, ErrorMessage = "El tiempo de recuperación entre series no puede exceder 10 caracteres")]
        public string RecoveryBetweenSets { get; set; } = "00:00";

        [Range(0, int.MaxValue, ErrorMessage = "El índice de orden de la serie no puede ser negativo")]
        public int OrderIndex { get; set; } = 0;

        [StringLength(1000, ErrorMessage = "Las notas de la serie no pueden exceder 1000 caracteres")]
        public string? Notes { get; set; }

        [Required]
        public List<CreateTrainingIntervalDto> Intervals { get; set; } = new();
    }

    /// <summary>
    /// DTO para crear un intervalo de entrenamiento
    /// </summary>
    public class CreateTrainingIntervalDto
    {
        [Required(ErrorMessage = "El tipo de intervalo es requerido")]
        public TrainingIntervalType Type { get; set; }

        [Required(ErrorMessage = "Las repeticiones son requeridas")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe haber al menos 1 repetición")]
        public int Repetitions { get; set; }

        [Required(ErrorMessage = "La distancia es requerida")]
        [Range(0, double.MaxValue, ErrorMessage = "La distancia no puede ser negativa")]
        public decimal Distance { get; set; }

        [StringLength(10, ErrorMessage = "El tiempo objetivo no puede exceder 10 caracteres")]
        public string? TargetTime { get; set; }

        [Required(ErrorMessage = "El tiempo de recuperación es requerido")]
        [StringLength(10, ErrorMessage = "El tiempo de recuperación no puede exceder 10 caracteres")]
        public string RecoveryTime { get; set; } = "00:00";

        [Required(ErrorMessage = "El tipo de ritmo es requerido")]
        public PaceType PaceType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El ritmo no puede ser negativo")]
        public decimal? Pace { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de VO2Max debe estar entre 0 y 100")]
        public decimal? Vo2MaxPercentage { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        public IntensityLevel? Intensity { get; set; }

        public TrainingMode? TrainingMode { get; set; }

        [StringLength(10, ErrorMessage = "La duración no puede exceder 10 caracteres")]
        public string? Duration { get; set; }

        [StringLength(10, ErrorMessage = "La velocidad objetivo no puede exceder 10 caracteres")]
        public string? TargetSpeed { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El índice de orden no puede ser negativo")]
        public int OrderIndex { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de plantilla de entrenamiento
    /// </summary>
    public class TrainingTemplateResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrainingType Type { get; set; }
        public TrainingCategory Category { get; set; }
        public int Duration { get; set; }
        public decimal? Distance { get; set; }
        public string? TargetPace { get; set; }
        public string? TargetHR { get; set; }
        public string Notes { get; set; } = string.Empty;
        public TrainingDifficulty Difficulty { get; set; }
        public bool IsFavorite { get; set; }
        public int UseCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsed { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Series completas de la plantilla (nueva estructura)
        /// </summary>
        public List<TrainingSeriesResponseDto> Series { get; set; } = new();

        /// <summary>
        /// Identifica cómo se creó la plantilla: "simple" (intervalos) o "advanced" (series)
        /// </summary>
        public string StructureType { get; set; } = "simple";
    }

    /// <summary>
    /// DTO para respuesta de serie de entrenamiento
    /// </summary>
    public class TrainingSeriesResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Repetitions { get; set; }
        public string RecoveryBetweenSets { get; set; } = "00:00";
        public int OrderIndex { get; set; }
        public string? Notes { get; set; }
        public List<TrainingIntervalResponseDto> Intervals { get; set; } = new();
    }

    /// <summary>
    /// DTO para respuesta de intervalo de entrenamiento
    /// </summary>
    public class TrainingIntervalResponseDto
    {
        public int Id { get; set; }
        public TrainingIntervalType Type { get; set; }
        public int Repetitions { get; set; }
        public decimal Distance { get; set; }
        public string? TargetTime { get; set; }
        public string RecoveryTime { get; set; } = "00:00";
        public PaceType PaceType { get; set; }
        public decimal? Pace { get; set; }
        public decimal? Vo2MaxPercentage { get; set; }
        public string? Description { get; set; }
        public IntensityLevel? Intensity { get; set; }
        public TrainingMode? TrainingMode { get; set; }
        public string? Duration { get; set; }
        public string? TargetSpeed { get; set; }
        public int OrderIndex { get; set; }
    }
}
