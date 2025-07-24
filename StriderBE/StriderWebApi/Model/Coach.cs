namespace StriderWebApi.Model;

public class Coach(User user)
{

    private User _user { get; } = user;

    private List<Team> _teams { get; } = [];

    public void AddTeam(Team team) => _teams.Add(team);

}