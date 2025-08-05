using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public abstract class Ailment
    {
        public int Id { get; set; }
        public AilmentSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string AdditionalNotes { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? RecoveryDate { get; set; }
        public bool IsRecovered { get; set; }
        
        // Navigation properties
        public int AthleteId { get; set; }
        public Athlete Athlete { get; set; } = null!;
        
        public abstract string GetName();
    }
    
    public class Injury : Ailment
    {
        public InjuryType Type { get; set; }
        public InjuryLocation Location { get; set; }
        
        public override string GetName()
        {
            return Type.ToString() + " " + Location.ToString();
        }
    }
    
    public class Illness : Ailment
    {
        public IllnessType Type { get; set; }
        
        public override string GetName()
        {
            return Type.ToString();
        }
    }
} 