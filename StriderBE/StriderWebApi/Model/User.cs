namespace StriderWebApi.Model;
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public List<Workout> Workouts { get; set; }

    public User(int id, string name, string password, string email)
    {
        Id = id;
        Name = name;
        Password = password;
        Email = email;
        Workouts = new List<Workout>();
    }

}