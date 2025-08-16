using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public abstract class Ailment
{
    public int Id { get; set; }
    public AilmentSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AdditionalNotes { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? RecoveryDate { get; set; }
    public DateTime? ExpectedRecoveryDate { get; set; }
    public bool IsRecovered => RecoveryDate != null;
    
    public abstract string GetName();

    public void FinishRecovery()
    {
        RecoveryDate = DateTime.Now;
        ExpectedRecoveryDate = null;
    }

    public void StartRecovery(DateTime expectedRecoveryDate)
    {
        RecoveryDate = null;
        ExpectedRecoveryDate = expectedRecoveryDate;
    }

}

public class Injury : Ailment
{
    public InjuryType Type { get; set; }
    public BodyPart Location { get; set; }
    
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
