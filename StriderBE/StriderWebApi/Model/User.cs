namespace StriderWebApi.Model;

public enum Gender
{
    MALE,
    FEMALE
}

public class User(int id, string username, string name, string password, string email, Gender gender, string address)
{
    public int Id { get; } = id;
    public string Username { get;  } = username;
    public string Name { get;  } = name;
    public string Password { get; } = password;
    public string Email { get;} = email;
    public Gender Gender { get;  } = gender;
    public string Address { get; } = address;

}