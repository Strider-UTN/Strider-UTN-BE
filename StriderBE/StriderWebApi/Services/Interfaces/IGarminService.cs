using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Dto.Garmin;

namespace StriderWebApi.Services.Interfaces;

public interface IGarminService
{

    Task<GarminLoginResponseDto> Login(Athlete athlete, GarminLoginRequestDto garminLoginRequestDto);

    Task<List<GarminWorkoutDto>> GetWorkouts(Athlete athlete, GarminWorkoutRequestDto garminWorkoutRequestDto);
}

