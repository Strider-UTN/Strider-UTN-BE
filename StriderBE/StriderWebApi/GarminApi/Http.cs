namespace StriderWebApi.GarminApi;
public interface IHttpClientHandler
{
    Task<HttpResponseMessage> GetAsync(string url);
    Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
    Task<HttpResponseMessage> DeleteAsync(string url);

}

public class HttpClientHandler : IHttpClientHandler
{

    private HttpClient Client { get; set; }

    public HttpClientHandler(string host, int port)
    {
        Client = new()
        {
            BaseAddress = new Uri($"http://{host}:{port}")
        };
    }

    public Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return Client.DeleteAsync(url);
    }

    public Task<HttpResponseMessage> GetAsync(string url)
    {
        return Client.GetAsync(url );
    }

    public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
    {
        return Client.PostAsync(url, content);
    }
}
