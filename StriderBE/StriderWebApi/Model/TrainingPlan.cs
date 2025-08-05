namespace StriderWebApi.Model;

public class TrainingPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }    
    public List<Session> Sessions { get; set; } = [];

    public void AddSession(Session session) => Sessions.Add(session);
    public void RemoveSession(Session session) => Sessions.Remove(session);
}