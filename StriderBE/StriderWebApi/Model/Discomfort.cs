using StriderWebApi.Domain.Enums;
namespace StriderWebApi.Model;

public class Discomfort
{
    public BodyPart? BodyPart { get; set; }
    public DiscomfortSource DiscomfortSource { get; set; }
    public required DiscomfortLevel Level { get; set; }
}

public enum DiscomfortLevel
{
    Mild,
    Moderate,
    Severe
}