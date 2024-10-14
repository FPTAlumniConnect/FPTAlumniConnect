using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.CV
{
    public class CVReponse
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string Type { get; set; } = null!;

        public string Cv1 { get; set; } = null!;
    }
}
