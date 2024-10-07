using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.PostReport
{
    public class PostReportReponse
    {
        public int RpId { get; set; }

        public int? PostId { get; set; }

        public int? UserId { get; set; }
    }
}
