namespace StriderWebApi.GarminApi;
public interface IHttpClientHandler
{
    Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string>? headers = null);
    Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Dictionary<string, string>? headers = null);
    Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string>? headers = null);

}

public class HttpClientHandler : IHttpClientHandler
{

    private HttpClient Client { get; set; }

    public HttpClientHandler(string garminApiUrl)
    {
        Client = new()
        {
            BaseAddress = new Uri(garminApiUrl)
        };
    }

    public Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        return Client.SendAsync(request);
    }

    public Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        return Client.SendAsync(request);
    }

    public Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        return Client.SendAsync(request);
    }
}
