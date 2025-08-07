namespace StriderWebApi.Exceptions.AccountActivation
{
    public class ActivationTokenInvalidOrExpiredException : Exception
    {
        public ActivationTokenInvalidOrExpiredException() : base() { }
        public ActivationTokenInvalidOrExpiredException(string message) : base(message) { }
        public ActivationTokenInvalidOrExpiredException(string message, Exception inner) : base(message, inner) { }
    }
}
