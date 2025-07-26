namespace StriderWebApi.Model;


public enum InjurySeverity
{
    MINOR,
    MODERATE,
    SERIOUS
}

public enum InjuryType
{
    TEAR,
    INJURY,
    SPRAIN
}

public enum InjuryLocation
{
    HIP,
    THIGH,
    KNEE,
    CALF,
    ANKLE,
    FOOT
}

public class InprogressInjury(InjuryData data, DateTime expectedRecoveryDate)
{
    public InjuryData Data = data;
    public DateTime ExpectedRecoveryDate = expectedRecoveryDate;
    public RecoveredInjury Recover(DateTime recoveryDate) => new(Data, recoveryDate);
}

public class RecoveredInjury(InjuryData data, DateTime recoveryDate)
{
    public InjuryData Data = data;
    public DateTime RecoveryDate = recoveryDate;
}

public class InjuryData(InjuryType type, InjuryLocation location, InjurySeverity severity, string description, string additionalNotes, string treatment)
{

    public InjuryType Type { get; set; } = type;
    public InjuryLocation Location { get; set; } = location;
    public InjurySeverity Severity { get; set; } = severity;
    public string Description { get; set; } = description;
    public string AdditionalNotes { get; set; } = additionalNotes;
    public string Treatment { get; set; } = treatment;
    public DateTime StartDate { get; set; } = DateTime.Now;

}