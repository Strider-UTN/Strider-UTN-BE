using System.ComponentModel.DataAnnotations.Schema;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Coach : User
    {
        // Collections to match model
        public List<Team> Teams { get; set; } = [];
        public List<TrainingPlan> TrainingPlans { get; set; } = [];
        public List<TrainingTemplate> Templates { get; set; } = [];
        public List<Athlete> Athletes { get; set; } = [];
    }
}
