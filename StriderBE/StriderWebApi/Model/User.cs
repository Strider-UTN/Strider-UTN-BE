namespace StriderWebApi.Model;

public enum Gender
{
    MALE,
    FEMALE
}

public class User(int id, string name, string password, string email, int age, int height, Gender gender, string address)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Password { get; } = password;
    public string Email { get; } = email;

    public int Age { get; } = age;

    public int Height { get; } = height;

    public Gender Gender { get; } = gender;
    public string Address { get; } = address;

    private List<Workout> Workouts { get; } = [];

    public void AddWorkouts(List<Workout> activities)
    {
        Workouts.AddRange(activities);
    }

    public int WorkoutCount()
    {
        return Workouts.Count;
    }
}