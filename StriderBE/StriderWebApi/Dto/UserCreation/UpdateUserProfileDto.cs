using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.UserCreation
{
    /// <summary>
    /// DTO para actualizar el perfil del usuario
    /// </summary>
    public class UpdateUserProfileDto
    {
        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Número de teléfono
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// URL de la imagen de perfil
        /// </summary>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Sobre ti / biografía breve
        /// </summary>
        public string? Bio { get; set; }

        // Campos específicos para atletas
        /// <summary>
        /// Altura en cm (solo para atletas)
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Peso en kg (solo para atletas)
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Nombre del contacto de emergencia (solo para atletas)
        /// </summary>
        public string? EmergencyContactName { get; set; }

        /// <summary>
        /// Teléfono del contacto de emergencia (solo para atletas)
        /// </summary>
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// Relación con el contacto de emergencia (solo para atletas)
        /// </summary>
        public string? EmergencyContactRelationship { get; set; }

        /// <summary>
        /// País (solo para atletas)
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// VO2Max - Velocidad máxima por km (solo para atletas)
        /// Formato: mm:ss (ejemplo: "03:30" para 3 minutos y 30 segundos por km)
        /// </summary>
        public string? VO2Max { get; set; }

        /// <summary>
        /// Fecha de inicio de entrenamiento - mes y año (solo para atletas)
        /// Formato: YYYY-MM (ejemplo: 2020-03)
        /// </summary>
        public string? TrainingStartDate { get; set; }

        /// <summary>
        /// Tipo de volumen de entrenamiento (solo para atletas)
        /// </summary>
        public Domain.Enums.TrainingVolumeType? TrainingVolumeType { get; set; }

        /// <summary>
        /// Volumen de entrenamiento en km (solo para atletas)
        /// </summary>
        public int? TrainingVolumeKm { get; set; }
    }
}

