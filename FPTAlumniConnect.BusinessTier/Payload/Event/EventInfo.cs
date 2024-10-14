﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.Event
{
    public class EventInfo
    {
        public string EventName { get; set; } = null!;

        public string? Img { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }  

        public DateTime? EndDate { get; set; }    

        public int? EventHolderId { get; set; }   

        public string? Location { get; set; }
    }

}