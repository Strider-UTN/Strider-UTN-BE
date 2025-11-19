using System;
using System.Collections.Generic;

namespace StriderWebApi.Dto.Garmin;

public class GarminWorkoutDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Distance { get; set; }
    public string Date { get; set; } = string.Empty;
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public List<GarminLapDto> Laps { get; set; } = [];
}

