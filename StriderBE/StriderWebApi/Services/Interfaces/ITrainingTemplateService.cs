using StriderWebApi.Dto.Trainings;

namespace StriderWebApi.Services.Interfaces
{
    public interface ITrainingTemplateService
    {
        Task<TrainingTemplateResponseDto?> CreateTrainingTemplateAsync(CreateTrainingTemplateDto dto, CancellationToken cancellationToken);
        Task<TrainingTemplateResponseDto?> GetTrainingTemplateByIdAsync(int templateId, CancellationToken cancellationToken);
        Task<List<TrainingTemplateResponseDto>> GetAllTrainingTemplatesAsync(CancellationToken cancellationToken);
        Task<bool> DeleteTrainingTemplateAsync(int templateId, CancellationToken cancellationToken);
        Task<TrainingTemplateResponseDto?> UpdateTrainingTemplateAsync(int id, CreateTrainingTemplateDto dto, CancellationToken cancellationToken);
        Task<TrainingTemplateResponseDto?> ToggleFavoriteTemplateAsync(int id, CancellationToken cancellationToken);
    }
}
