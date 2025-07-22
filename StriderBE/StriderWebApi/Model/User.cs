namespace StriderWebApi.Model;

public enum Gender
{
    MALE,
    FEMALE
}

public class User(int id, string username, string name, string password, string email, Gender gender, string address)
{
    private int _id = id;
    private string _username = username;
    private string _name = name;
    private string _password = password;
    private string _email = email;
    private Gender _gender = gender;
    private string _address = address;

    public int Id => _id;

}