namespace StriderWebApi.Dto.Coach;

public class CoachFeedbackDTO
{
    public string Feedback { get; set; } = string.Empty;
    public Dictionary<int, string> LapFeedbacks { get; set; } = [];
}