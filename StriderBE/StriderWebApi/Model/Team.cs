namespace StriderWebApi.Model;

using StriderWebApi.Model;

public class Team(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    private List<User> Users { get; } = [];

    private List<Coach> Coaches { get; } = [];

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public void AddCoach(Coach coach)
    {
        Coaches.Add(coach);
    }

    public void RemoveUser(User user)
    {
        Users.Remove(user);
    }

    public void RemoveCoach(Coach coach)
    {
        Coaches.Remove(coach);
    }

    
}