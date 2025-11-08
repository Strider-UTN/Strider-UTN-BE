using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Trainings
{
    public class CreateTrainingSessionDto
    {
        public int PlanningId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TrainingCategory Category { get; set; } = TrainingCategory.Training;
        public IEnumerable<int> AthleteIds { get; set; } = new List<int>();

        /// <summary>
        /// Nueva estructura basada en series. Preferir este campo.
        /// </summary>
        public List<CreateTrainingSeriesDto>? Series { get; set; }

        public string? Notes { get; set; }
    }

    // Actualización de UpdateTrainingSessionDto
    public class UpdateTrainingSessionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TrainingCategory Category { get; set; }
        public IEnumerable<int> AthleteIds { get; set; } = new List<int>();

        /// <summary>
        /// Nueva estructura basada en series. Preferir este campo.
        /// </summary>
        public List<CreateTrainingSeriesDto>? Series { get; set; }

        public string? Notes { get; set; }
    }

    // Actualización de TrainingSessionResponseDto para incluir volumen calculado e intervalos
    public class TrainingSessionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TrainingCategory Category { get; set; }
        public int PlanningId { get; set; }
        public int MicrocycleId { get; set; }
        public IEnumerable<int> AthleteIds { get; set; } = new List<int>();

        /// <summary>
        /// Series completas de la sesión.
        /// </summary>
        public List<TrainingSeriesResponseDto> Series { get; set; } = new();

        /// <summary>
        /// Indica si la sesión se construyó con intervalos simples o series avanzadas.
        /// </summary>
        public string StructureType { get; set; } = "simple";

        public string? Notes { get; set; }
        public decimal Volume { get; set; } // Volumen calculado desde intervalos (en km)
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // DTO auxiliar
    public class AssignAthletesDto
    {
        public IEnumerable<int> AthleteIds { get; set; } = new List<int>();
    }
}
