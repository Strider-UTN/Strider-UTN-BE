using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    /// <summary>
    /// Entidad que representa un punto de entrenamiento (ubicación)
    /// perteneciente a una sede
    /// </summary>
    [Table("TrainingPoints")]
    public class TrainingPoint
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Relación con la sede
        [Required]
        public int TrainingGroupId { get; set; }

        [ForeignKey(nameof(TrainingGroupId))]
        public TrainingGroup TrainingGroup { get; set; } = null!;

        // Opcional: coordenadas geográficas para futuras funcionalidades de mapas
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        // Fecha de creación del punto
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
