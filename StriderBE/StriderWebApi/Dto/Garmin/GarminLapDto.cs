using System;

namespace StriderWebApi.Dto.Garmin;

public class GarminLapDto
{
    public int Index { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public double Speed { get; set; }
    public string StartTime { get; set; } = string.Empty;
}

