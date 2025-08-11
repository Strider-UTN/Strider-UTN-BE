using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Model;

public class Session
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Warmup { get; set; } = string.Empty;
    public string CoolDown { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public WorkoutType SessionType { get; set; }
    public List<Serie> Series { get; set; } = [];

    public double TotalDistance(Athlete athlete) => Series.Sum(l => l.TotalDistance(athlete));
    public Session Clone() => new() 
    { 
        Date = Date, 
        Warmup = Warmup, 
        CoolDown = CoolDown, 
        Description = Description, 
        Name = Name, 
        Category = Category, 
        Label = Label, 
        Comments = Comments, 
        Series = new List<Serie>(Series.Select(i => i.Clone())), 
        SessionType = SessionType 
    };
    
}
