using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.JobPost
{
    public class JobPostInfo
    {
        public string JobDescription { get; set; } = null!;

        public string? Salary { get; set; }

        public string? Location { get; set; }

        public string? Requirements { get; set; }

        public string? Benefits { get; set; }

        public DateTime Time { get; set; }

        public string Status { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int? UserId { get; set; }

        public int? MajorId { get; set; }
    }

}
