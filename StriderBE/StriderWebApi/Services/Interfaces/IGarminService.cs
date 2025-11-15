using StriderWebApi.Domain.DomainClasses;

namespace StriderWebApi.Services.Interfaces;

public interface IGarminService
{
    Task DeleteUser(Athlete athlete);
    Task<List<GarminWorkout>> GetWorkouts(Athlete athlete, DateTime start, DateTime end, string garminName, string garminPassword);
}

