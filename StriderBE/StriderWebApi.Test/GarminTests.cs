using System.Net;
using Moq;
using StriderWebApi.GarminApi;
using StriderWebApi.Model;

public class GarminTests
{

    private Mock<IHttpClientHandler> _mockService;

    private string _sampleResponse = @"[
            {
                ""id"": 1,
                ""name"": ""Test workout"",
                ""distance"": 1000,
                ""date"": ""2022-01-01T00:00:00Z"",
                ""duration"": 3600,
                ""laps"": [
                    {
                        ""index"": 1,
                        ""startTime"": ""2022-01-01T00:00:00Z"",
                        ""distance"": 1000,
                        ""duration"": 3600,
                        ""averageSpeed"": 10
                    }
                ]
            }
        ]   ";

    public GarminTests()
    {
        _mockService = new Mock<IHttpClientHandler>();
    }

    [Fact]
    public async Task TestUserRegistration()
    {
        _mockService.Setup(m => m.PostAsync("users/1", It.IsAny<HttpContent>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        GarminInterface garminInterface = new GarminInterface("localhost", 8080, _mockService.Object);
        User user = new User(1, "test", "test", "test");
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
        User user = new(1, "test", "test", "test");
        await Assert.ThrowsAsync<Exception>(() => garminInterface.RegisterUser(user, "test", "test"));
    }

    [Fact]
    public async Task TestUserDeletion()
    {
        _mockService.Setup(m => m.DeleteAsync("users/1")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        User user = new(1, "test", "test", "test");
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
        User user = new(1, "test", "test", "test");
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
        User user = new(1, "test", "test", "test");
        await garminInterface.GetWorkouts(user, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2));

        Assert.Single(user.Workouts);

    }

    [Fact]
    public async Task TestGettingWorkoutsFailure()
    {
        _mockService.Setup(m => m.GetAsync("users/1/workouts?start_date=2022-01-01&end_date=2022-01-02")).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"message\":\"Test message\"}", System.Text.Encoding.UTF8, "application/json")
        }));
        GarminInterface garminInterface = new("localhost", 8080, _mockService.Object);
        User user = new(1, "test", "test", "test");
        await Assert.ThrowsAsync<Exception>(() => garminInterface.GetWorkouts(user, new DateTime(2022, 1, 1), new DateTime(2022, 1, 2)));
    }


}