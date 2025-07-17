namespace StriderWebApi.Test;

using System.Net;
using Moq;
using StriderWebApi.GarminApi;
using StriderWebApi.Model;

public class GarminTests
{

    private readonly Mock<IHttpClientHandler> _mockService;

    private readonly User user = new(1, "test", "test", "test",22,186,Gender.MALE,"Address");

    private string _sampleResponse = @"
    {
        ""Data"": [
            {
                ""Id"": 1,
                ""Name"": ""Test Workout"",
                ""Distance"": 1000,
                ""Date"": ""2022-01-01T00:00:00Z"",
                ""Duration"": 3600,
                ""Laps"": [
                    {
                        ""Index"": 1,
                        ""StartTime"": ""2022-01-01T00:00:00Z"",
                        ""Distance"": 1000,
                        ""Duration"": 3600,
                        ""AverageSpeed"": 10
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
        GarminInterface garminInterface = new GarminInterface("localhost", 8080, _mockService.Object);
        await garminInterface.RegisterUser(user, "test", "test");
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
        await Assert.ThrowsAsync<Exception>(() => garminInterface.RegisterUser(user, "test", "test"));
    }

    [Fact]
    public async Task TestUserDeletion()
    {
        _mockService.Setup(m => m.DeleteAsync("users/1")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        await garminInterface.DeleteUser(user);
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
        await Assert.ThrowsAsync<Exception>(() => garminInterface.DeleteUser(user));
    }

    [Fact]
    public async Task TestGettingWorkouts()
    {

        _mockService.Setup(m => m.GetAsync("users/1/workouts?start_date=2022-01-01&end_date=2022-01-02")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_sampleResponse, System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        List<Workout> workouts = await garminInterface.GetWorkouts(user, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));
        user.AddWorkouts(workouts);
        Assert.Equal(1,user.WorkoutCount());

    }

    [Fact]
    public async Task TestGettingWorkoutsFailure()
    {
        _mockService.Setup(m => m.GetAsync("users/1/workouts?start_date=2022-01-01&end_date=2022-01-02")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
       
        await Assert.ThrowsAsync<Exception>(() => garminInterface.GetWorkouts(user, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2)));
    }


}