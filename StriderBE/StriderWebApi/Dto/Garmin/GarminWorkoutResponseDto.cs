using System.Collections.Generic;

namespace StriderWebApi.Dto.Garmin;

public class GarminWorkoutResponseDto
{
    public List<GarminWorkoutDto> Workouts { get; set; } = [];
}

