using System;
using System.Collections.Generic;

namespace StriderWebApi.Domain.DomainClasses;

public class GarminWorkout
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Distance { get; set; }
    public DateTime Date { get; set; }
    public double Duration { get; set; }
    public double AverageHR { get; set; }
    public List<GarminLap> Laps { get; set; } = [];
}

