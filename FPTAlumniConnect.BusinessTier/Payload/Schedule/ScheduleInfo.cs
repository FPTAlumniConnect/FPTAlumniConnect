using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Schedule;

public class ScheduleInfo
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Content { get; set; }
}
