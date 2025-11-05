using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Dto.Planning
{
    public class UpdatePlanningDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PlanningStatus Status { get; set; }
    }
}
