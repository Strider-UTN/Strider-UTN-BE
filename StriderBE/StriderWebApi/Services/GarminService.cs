using System.Net;
using System.Text;
using System.Text.Json;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.GarminApi.DTOs;

namespace StriderWebApi.GarminApi;

public class GarminService(string host, int port, IHttpClientHandler? client) : IGarminService
{


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
    public async Task DeleteUser(Athlete user)
    {
        var token = GetAPIToken();
        var headers = new Dictionary<string, string>
        {
            { "X-API-Key", token }
        };
        var response = await _client.DeleteAsync("users/" + user.Id, headers);
        await HandleError(response);
    }

#pragma warning disable CS8604
    public async Task<List<GarminWorkout>> GetWorkouts(Athlete user, DateTime start, DateTime end, string garminName, string garminPassword)
    {

        var requestData = new
        {
            start_date = start.ToString("yyyy-MM-dd"),
            end_date = end.ToString("yyyy-MM-dd"),
            garmin_name = garminName,
            garmin_password = garminPassword
        };
        var content = new StringContent(JsonSerializer.Serialize(requestData), System.Text.Encoding.UTF8, "application/json");
        var token = GetAPIToken();
        var headers = new Dictionary<string, string>
        {
            { "X-API-Key", token }
        };
        HttpResponseMessage result = await _client.PostAsync($"users/{user.Id}/workouts", content, headers);

        await HandleError(result);

        var response = JsonSerializer.Deserialize<APIResponse>(await result.Content.ReadAsStringAsync()) ?? throw new Exception("No activities found");
        object Data = response.Data ?? throw new Exception("No activities found");

        var activities = JsonSerializer.Deserialize<List<GarminWorkout>>(Data.ToString()) ?? throw new Exception("No activities found");

        return activities;
        
    }

    private static string GetAPIToken(){
        string apiToken = Environment.GetEnvironmentVariable("GARMIN_API_KEY") ?? throw new Exception("GARMIN_API_KEY environment variable is not set");
        var sha256 = System.Security.Cryptography.SHA256.Create();
        var tokenBytes = Encoding.UTF8.GetBytes(apiToken);
        var hashBytes = sha256.ComputeHash(tokenBytes);
        var hashedToken = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hashedToken;
    }

}

