namespace StriderWebApi.Model;

public class Workout(int id, string name, int distance, DateTime date, TimeSpan duration, List<Lap> laps)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;

    public int Distance { get; set; } = distance;

    public DateTime Date { get; set; } = date;

    public TimeSpan Duration { get; set; } = duration;

    public List<Lap> Laps { get; set; } = laps;
}
