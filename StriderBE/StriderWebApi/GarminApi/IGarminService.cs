using StriderWebApi.Model;

namespace StriderWebApi.Services.Interfaces;

public interface IGarminService
{
    Task DeleteUser(User user);
    Task<List<Workout>> GetWorkouts(Athlete user, DateTime start, DateTime end, string garminName, string garminPassword);
}

