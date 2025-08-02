namespace StriderWebApi.Exceptions.Coach;
public class CoachNotFoundException : Exception
{
    public CoachNotFoundException()
    {
    }

    public CoachNotFoundException(string? message) : base(message)
    {
    }

    public CoachNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}