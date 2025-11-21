namespace StriderWebApi.Exceptions.Athlete
{
    [Serializable]
    public class AthleteNotFoundException : Exception
    {
        public AthleteNotFoundException()
        {
        }

        public AthleteNotFoundException(string? message) : base(message)
        {
        }

        public AthleteNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
