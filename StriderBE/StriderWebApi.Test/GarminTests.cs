namespace StriderWebApi.Test;

using System.Net;
using Moq;
using StriderWebApi.GarminApi;
using StriderWebApi.Model;
using StriderWebApi.Domain.Enums;

public class GarminTests
{

    private readonly Mock<IHttpClientHandler> _mockService;

    private readonly Athlete _user = new() 
    { 
        Id = 1, 
        Username = "Test User", 
        Name = "test", 
        Email = "test", 
        Gender = Gender.MALE, 
        Address = "test", 
        VO2Max = 0, 
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
    public async Task TestUserRegistration()
    {
        _mockService.Setup(m => m.PostAsync("users/1", It.IsAny<HttpContent>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        await garminInterface.RegisterUser(_user, "test", "test");
        _mockService.Verify(m => m.PostAsync("users/1", It.IsAny<HttpContent>()), Times.Once);
    }

    [Fact]
    public async Task TestUserRegistrationFailure()
    {
        _mockService.Setup(m => m.PostAsync("users/1", It.IsAny<HttpContent>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        await Assert.ThrowsAsync<Exception>(() => garminInterface.RegisterUser(_user, "test", "test"));
    }

    [Fact]
    public async Task TestUserDeletion()
    {
        _mockService.Setup(m => m.DeleteAsync("users/1")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        await garminInterface.DeleteUser(_user);
        _mockService.Verify(m => m.DeleteAsync("users/1"), Times.Once);
    }

    [Fact]
    public async Task TestUserDeletionFailure()
    {
        _mockService.Setup(m => m.DeleteAsync("users/1")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        await Assert.ThrowsAsync<Exception>(() => garminInterface.DeleteUser(_user));
    }

    [Fact]
    public async Task TestGettingWorkouts()
    {

        _mockService.Setup(m => m.GetAsync("users/1/workouts?start_date=2022-01-01&end_date=2022-01-02")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_sampleResponse, System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        List<Workout> workouts = await garminInterface.GetWorkouts(_user, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.Equal(3600, workouts[0].Laps[0].Duration);
    }

    [Fact]
    public async Task TestGettingWorkoutsFailure()
    {
        _mockService.Setup(m => m.GetAsync("users/1/workouts?start_date=2022-01-01&end_date=2022-01-02")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
       
        await Assert.ThrowsAsync<Exception>(() => garminInterface.GetWorkouts(_user, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2)));
    }


}