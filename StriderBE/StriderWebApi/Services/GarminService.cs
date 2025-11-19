using System.Net;
using System.Text;
using System.Text.Json;
using StriderWebApi.Services.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.GarminApi.DTOs;
using StriderWebApi.Dto.Garmin;
using System.Security.Principal;

namespace StriderWebApi.GarminApi;

public class GarminService(IHttpClientHandler client, string token) : IGarminService
{

    private readonly IHttpClientHandler _client = client;
    private readonly string _token = token;

    public Task DeleteUser(Athlete athlete)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GarminWorkoutDto>> GetWorkouts(Athlete user, GarminWorkoutRequestDto garminWorkoutRequestDto)
    {
        var json = JsonSerializer.Serialize(garminWorkoutRequestDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {_token}" }
        };
        HttpResponseMessage result = await _client.PostAsync($"users/{user.Id}/workouts", content, headers);
        if (!result.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get workouts");
        }
        var data = JsonSerializer.Deserialize<APIResponse>(await result.Content.ReadAsStringAsync()) ?? throw new Exception("Failed to deserialize data");
        if (data.Data == null)
        {
            throw new Exception("No data found");
        }
        var workouts = JsonSerializer.Deserialize<GarminWorkoutResponseDto>(data.Data.ToString() ?? throw new Exception("Failed to deserialize workouts")) ?? throw new Exception("Failed to deserialize workouts");
        return workouts.Workouts;
    }

    public async Task<GarminLoginResponseDto> Login(Athlete athlete, GarminLoginRequestDto garminLoginRequestDto)
    {   
        var json = JsonSerializer.Serialize(garminLoginRequestDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {_token}" }
        };
        HttpResponseMessage result = await _client.PostAsync($"users/{athlete.Id}/login", content, headers);
        var response = JsonSerializer.Deserialize<GarminLoginResponseDto>(await result.Content.ReadAsStringAsync()) ?? throw new Exception("No response from Garmin");
        return response;
    }

}
