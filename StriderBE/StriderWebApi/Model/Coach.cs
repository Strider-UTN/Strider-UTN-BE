namespace StriderWebApi.Model;

public class Coach(string name, string address)
{
    private string Name { get;} = name;
    private string Address { get;  } = address;

    private List<Team> Teams { get; } = [];


    public void AddTeam(Team team)
    {
        this.Teams.Add(team);
    }

}