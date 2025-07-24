using System.Net;
using System.Text.Json;
using StriderWebApi.Model;

namespace StriderWebApi.GarminApi;



public class GarminInterface(string host, int port, IHttpClientHandler? client)
{

    class APIResponse
    {
        public string ? Message { get; set; }
        public object? Data { get; set; }
    }

    private readonly IHttpClientHandler _client = client ?? new HttpClientHandler(host, port);

    private static async Task HandleError(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.OK)
        {
            APIResponse? jsonContent = await response.Content.ReadFromJsonAsync<APIResponse>();
            var message = jsonContent?.Message;
            throw new Exception("API Responded with error: " + message);
        }
    }

    public async Task RegisterUser(User user, string garminUserName, string garminPassword, string mfaToken = "")
    {
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            garmin_username = garminUserName,
            garmin_password = garminPassword,
            garmin_mfa_code = mfaToken
        }), System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("users/" + user.Id, content);
        await HandleError(response);
    }

    public async Task DeleteUser(User user)
    {
        var response = await _client.DeleteAsync("users/" + user.Id);
        await HandleError(response);
    }

#pragma warning disable CS8604
    public async Task<List<Workout>> GetWorkouts(User user, DateTime start, DateTime end)
    {

        HttpResponseMessage result = await _client.GetAsync("users/" + user.Id + "/workouts?" + "start_date=" + start.ToString("yyyy-MM-dd") + "&end_date=" + end.ToString("yyyy-MM-dd"));

        await HandleError(result);

        var response = JsonSerializer.Deserialize<APIResponse>(await result.Content.ReadAsStringAsync()) ?? throw new Exception("No activities found");
        object Data = response.Data ?? throw new Exception("No activities found");

        var activities = JsonSerializer.Deserialize<List<Workout>>(Data.ToString()) ?? throw new Exception("No activities found");

        return activities;
        
    }

}