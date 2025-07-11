namespace StriderWebApi.Model;

public class Workout
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int Distance { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan Duration { get; set; }

    public List<Lap> Laps { get; set; }

    public Workout(int id, string name, int distance, DateTime date, TimeSpan duration, List<Lap> laps)
    {
        Id = id;
        Name = name;
        Distance = distance;
        Date = date;
        Duration = duration;
        Laps = laps;
    }
}
