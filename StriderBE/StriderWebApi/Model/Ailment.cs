namespace StriderWebApi.Model;

public enum AilmentSeverity
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

public enum IllnessType
{
    FLU,
    PNEUMONIA,
    DIARRHEA,
    COLD,
    VOMITING,
    HEADACHE,
    CHEST_PAIN,
    STOMACH_PAIN
}

public class Ailment(AilmentSeverity severity, string description, string additionalNotes, string treatment, IAilmentState state)
{
    private readonly AilmentSeverity _severity = severity;
    private readonly string _description = description;
    private readonly string _additionalNotes = additionalNotes;
    private readonly string _treatment = treatment;
    private readonly DateTime _startDate = DateTime.Now;
    private readonly IAilmentState _state = state;

    public AilmentSeverity Severity => _severity;
    public string Description => _description;
    public string AdditionalNotes => _additionalNotes;
    public string Treatment => _treatment;
    public DateTime StartDate => _startDate;
    public IAilmentState State => _state;

    public bool IsRecovered()
    {
        return _state.IsRecovered();
    }
}

public interface IAilmentState
{
    public bool IsRecovered();
}

public class InProgress(DateTime expectedEndTime) : IAilmentState
{
    private readonly DateTime _expectedEndTime = expectedEndTime;

    public DateTime ExpectedEndTime => _expectedEndTime;

    bool IAilmentState.IsRecovered()
    {
        return false;
    }
}

public class Recovered(DateTime recoveryDate) : IAilmentState
{
    private readonly DateTime _recoveryDate = recoveryDate;

    public DateTime RecoveryDate => _recoveryDate;

    bool IAilmentState.IsRecovered()
    {
        return true;
    }
}

public class Injury(InjuryType injuryType, InjuryLocation location, AilmentSeverity severity, string description, string additionalNotes, string treatment, IAilmentState state) : Ailment(severity, description, additionalNotes, treatment, state)
{
    private InjuryType _type = injuryType;
    private InjuryLocation _location = location;

    public InjuryType Type 
    { 
        get => _type; 
        set => _type = value; 
    }
    
    public InjuryLocation Location 
    { 
        get => _location; 
        set => _location = value; 
    }
}

public class Illness(IllnessType type, AilmentSeverity severity, string description, string additionalNotes, string treatment, IAilmentState state) : Ailment(severity, description, additionalNotes, treatment, state)
{
    private IllnessType _type = type;

    public IllnessType Type 
    { 
        get => _type; 
        set => _type = value; 
    }
}
