using System.ComponentModel.DataAnnotations.Schema;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Domain.DomainClasses
{
    public class Interval
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public double Speed { get; set; }
        public int Percentage { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        
        // Navigation property
        public int SessionId { get; set; }
        public Session Session { get; set; } = null!;
    }
} 