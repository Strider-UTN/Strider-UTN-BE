namespace StriderWebApi.Test;

using System.Net;
using Moq;
using StriderWebApi.GarminApi;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;
using StriderWebApi.Dto.Garmin;

public class GarminTests
{

    private readonly Mock<IHttpClientHandler> _mockService;

    private readonly Athlete _user = new() 
    { 
        Id = 1, 
        Username = "Test User", 
        FullName = "Test User", 
        Email = "test", 
        Gender = Gender.MALE, 
        Address = "test", 
        //VO2Max = 0, 
        MedicalConditions = new(), 
        Objectives = new(),
        CreatedBy = "test",
        BirthDate = DateTime.Now 
    };

    private string _sampleResponse = @"
    {
        ""Data"": [
            {
                ""Id"": 1,
                ""Name"": ""Test Workout"",
                ""Distance"": 1000,
                ""Date"": ""2022-01-01T00:00:00Z"",
                ""Duration"": 3600,
                ""AverageHR"": 100,
                ""Laps"": [
                    {
                        ""Index"": 1,
                        ""StartTime"": ""2022-01-01T00:00:00Z"",
                        ""Distance"": 1000,
                        ""Duration"": 3600,
                        ""Speed"": 10,
                        ""AverageHR"": 100
                    }
                ]
            }
        ],
        ""Message"": ""Workouts fetched succesfully""
    }";

    public GarminTests()
    {
        _mockService = new Mock<IHttpClientHandler>();
    }

    [Fact]
    public async Task TestUserDeletion()
    {
        _mockService.Setup(m => m.DeleteAsync("users/1", It.IsAny<Dictionary<string, string>?>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        GarminService garminService = new(_mockService.Object, "token_test");
        await garminService.DeleteUser(_user);
        _mockService.Verify(m => m.DeleteAsync("users/1", It.IsAny<Dictionary<string, string>?>()), Times.Once);
    }

    [Fact]
    public async Task TestUserDeletionFailure()
    {
        _mockService.Setup(m => m.DeleteAsync("users/1", It.IsAny<Dictionary<string, string>?>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminService garminService = new(_mockService.Object, "token_test");
        await Assert.ThrowsAsync<Exception>(() => garminService.DeleteUser(_user));
    }

    [Fact]
    public async Task TestGettingWorkouts()
    {

        _mockService.Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<Dictionary<string, string>?>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_sampleResponse, System.Text.Encoding.UTF8, "application/json")
        }));
        GarminService garminService = new(_mockService.Object, "token_test");
        List<GarminWorkoutDto> workouts = await garminService.GetWorkouts(_user, new GarminWorkoutRequestDto()
        {
            StartDate = new DateTime(2022, 1, 1),
            EndDate = new DateTime(2022, 1, 2)
        });

        Assert.Equal(3600, workouts[0].Laps[0].Duration);
    }

    [Fact]
    public async Task TestGettingWorkoutsFailure()
    {
        _mockService.Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<Dictionary<string, string>?>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminService garminService = new(_mockService.Object, "token_test");
       
        await Assert.ThrowsAsync<Exception>(() => garminService.GetWorkouts(_user, new GarminWorkoutRequestDto()
        {
            StartDate = new DateTime(2022, 1, 1),
            EndDate = new DateTime(2022, 1, 2)
        }));
    }


}