using MailKit;

public class Discomfort
{
    public required BodyPart BodyPart { get; set; }
    public required DiscomfortLevel Level { get; set; }
}

public enum DiscomfortLevel
{
    Mild,
    Moderate,
    Severe
}