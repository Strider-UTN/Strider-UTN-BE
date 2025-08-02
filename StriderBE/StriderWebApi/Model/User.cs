namespace StriderWebApi.Model;

public enum Gender
{
    MALE,
    FEMALE
}

public class User(int id, string username, string name, string password, string email, Gender gender, string address)
{
    private readonly int _id = id;
    private readonly string _username = username;
    private readonly string _name = name;
    private readonly string _password = password;
    private readonly string _email = email;
    private readonly Gender _gender = gender;
    private readonly string _address = address;

    public int Id => _id;
    public string Username => _username;
    public string Name => _name;
    public string Password => _password;
    public string Email => _email;
    public Gender Gender => _gender;
    public string Address => _address;
}