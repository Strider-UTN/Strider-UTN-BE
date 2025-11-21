namespace StriderWebApi.Dto.Athlete;

public class AthleteAnalysisResultResponseDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public AthleteAnalysisResultType Type { get; set; }
}

