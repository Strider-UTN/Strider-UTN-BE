namespace StriderWebApi.Dto.Coach;

public class CoachFeedbackDTO(string feedback, Dictionary<int, string> lapFeedbacks)
{
    public string Feedback { get; set; } = feedback;

    public Dictionary<int, string> LapFeedbacks { get; set; } = lapFeedbacks;

}