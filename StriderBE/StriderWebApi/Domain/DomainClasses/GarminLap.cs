using System;

namespace StriderWebApi.Domain.DomainClasses;

public class GarminLap
{
    public int Index { get; set; }
    public double Distance { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public double Speed { get; set; }
    public DateTime StartTime { get; set; }
}

