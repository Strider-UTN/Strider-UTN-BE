using StriderWebApi.Dto.Trainings;

namespace StriderWebApi.Services.Interfaces
{
    public interface ITrainingSessionsService
    {
        Task<TrainingSessionResponseDto> CreateTrainingSessionAsync(CreateTrainingSessionDto dto, int userId, CancellationToken cancellationToken = default);
        Task<TrainingSessionResponseDto?> GetTrainingSessionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<TrainingSessionResponseDto>> GetAllTrainingSessionsAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<TrainingSessionResponseDto>> GetTrainingSessionsByDateAsync(DateTime date, int userId, CancellationToken cancellationToken = default);
    }
}
