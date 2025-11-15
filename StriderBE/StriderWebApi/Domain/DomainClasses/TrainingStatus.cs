namespace StriderWebApi.Domain.DomainClasses;

using StriderWebApi.Domain.Enums;

public class TrainingStatus {

    public required string Title { get; set; }
    public required string Description { get; set; }
    public TrainingStatusType Type { get; set; }
    
}

