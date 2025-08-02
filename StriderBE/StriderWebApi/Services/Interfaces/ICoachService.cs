using StriderWebApi.Model;

namespace StriderWebApi.Services.Interfaces;


public interface ICoachService
{
    Task<Coach> GetCoachByIdAsync(int coachId);
}


