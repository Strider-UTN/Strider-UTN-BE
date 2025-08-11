

namespace StriderWebApi.Model;

public class Serie
{
    public List<Interval> Intervals { get; set; } = [];
    public int Repetitions { get; set; }
    public double Rest { get; set; }

    public Serie Clone()
    {
        return new()
        {
            Intervals = Intervals.Select(i => i.Clone()).ToList(),
            Repetitions = Repetitions,
            Rest = Rest
        };
    }

    public double TotalDistance(Athlete athlete) => Intervals.Sum(l => l.GetDistance(athlete));

}