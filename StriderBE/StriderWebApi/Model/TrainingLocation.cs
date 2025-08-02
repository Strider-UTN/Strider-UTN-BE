
namespace StriderWebApi.Model;

public readonly struct TrainingLocation(string address)
{
    private readonly string _address = address;

    public readonly string Address => _address;
}