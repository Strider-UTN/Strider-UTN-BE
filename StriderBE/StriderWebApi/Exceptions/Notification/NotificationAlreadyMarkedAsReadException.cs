namespace StriderWebApi.Exceptions.Notification
{
    public class NotificationAlreadyMarkedAsReadException : Exception
    {
        public NotificationAlreadyMarkedAsReadException() : base() { }
        public NotificationAlreadyMarkedAsReadException(string message) : base(message) { }
        public NotificationAlreadyMarkedAsReadException(string message, Exception inner) : base(message, inner) { }
    }
}
