using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Dto.Athlete
{
    public class UpdateAthleteStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
