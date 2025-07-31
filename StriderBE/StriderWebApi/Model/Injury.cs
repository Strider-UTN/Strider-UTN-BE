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

public class Injury(InjuryType type, InjuryLocation location, InjurySeverity severity, string description, string additionalNotes, string treatment)
{
    public InjuryType Type { get; set; } = type;
    public InjuryLocation Location { get; set; } = location;
    public InjurySeverity Severity { get; set; } = severity;
    public string Description { get; set; } = description;
    public string AdditionalNotes { get; set; } = additionalNotes;
    public string Treatment { get; set; } = treatment;
    public DateTime StartDate { get; set; } = DateTime.Now;
}

public class InprogressInjury(InjuryType type, InjuryLocation location, InjurySeverity severity, string description, string additionalNotes, string treatment, DateTime expectedRecoveryDate) : Injury(type, location, severity, description, additionalNotes, treatment)
{
    public DateTime ExpectedRecoveryDate { get; set; } = expectedRecoveryDate;

    public RecoveredInjury Recover(DateTime recoveryDate)
    {
        return new RecoveredInjury(Type, Location, Severity, Description, AdditionalNotes, Treatment, recoveryDate);
    }
}

public class RecoveredInjury(InjuryType type, InjuryLocation location, InjurySeverity severity, string description, string additionalNotes, string treatment, DateTime recoveryDate) : Injury(type, location, severity, description, additionalNotes, treatment)
{
    public DateTime RecoveryDate { get; set; } = recoveryDate;
}
