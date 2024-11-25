using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class CVFilter
    {
        public int? UserId { get; set; }

        public string? Address { get; set; } = null!;

        public string? Gender { get; set; } = null!;

        public string? Email { get; set; } = null!;

        public string? City { get; set; } = null!;

        public string? Company { get; set; } = null!;

        public string? PrimaryDuties { get; set; } = null!;

        public string? JobLevel { get; set; } = null!;

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public string? Language { get; set; } = null!;

        public string? LanguageLevel { get; set; } = null!;

        public string? MinSalary { get; set; } = null!;

        public string? MaxSalary { get; set; } = null!;

        public bool? IsDeal { get; set; }
    }
}
