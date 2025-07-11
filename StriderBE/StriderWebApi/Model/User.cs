namespace StriderWebApi.Model;
public class User(int id, string name, string password, string email)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Password { get; set; } = password;
    public string Email { get; set; } = email;

    public List<Workout> Workouts { get; set; } = new List<Workout>();
}