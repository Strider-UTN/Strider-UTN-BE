namespace StriderWebApi.Model;

public class Calendar(string name)
{
    private string _name { get; } = name;
    private List<Session> _sessions { get; } = new();

    public void AddSession(Session session)
    {
        _sessions.Add(session);
    }

    public void RemoveSession(Session session)
    {
        _sessions.Remove(session);
    }

    public void AddLabel(int weekStart, int weekEnd, string label)
    {
        _sessions.ForEach((s) =>
        {
            if (s.Week >= weekStart && s.Week <= weekEnd)
            {
                s.Label(label);
            }
        });
    }

}