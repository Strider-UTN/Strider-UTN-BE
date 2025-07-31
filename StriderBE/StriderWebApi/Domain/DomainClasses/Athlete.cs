using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Athlete : User
    {
        public DateTime BirthDate { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public string Country { get; set; } = string.Empty;
        public double? VO2Max { get; set; }
    }
}
