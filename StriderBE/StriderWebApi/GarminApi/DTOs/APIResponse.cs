namespace StriderWebApi.GarminApi.DTOs
{
    public class APIResponse(string? message, object? data)
    {
        public string? Message { get; } = message;
        public object? Data { get; } = data;
    }
}
