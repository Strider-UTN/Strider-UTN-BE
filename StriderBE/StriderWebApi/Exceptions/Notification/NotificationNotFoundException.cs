namespace StriderWebApi.Exceptions.Notification
{
    public class NotificationNotFoundException : Exception
    {
        public NotificationNotFoundException() : base() { }
        public NotificationNotFoundException(string message) : base(message) { }
        public NotificationNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
