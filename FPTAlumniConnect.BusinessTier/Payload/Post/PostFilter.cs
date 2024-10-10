﻿using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Post
{
    public class PostFilter
    {
        public int? AuthorId { get; set; }

        public string Content { get; set; } = null!;

        public string Title { get; set; } = null!;

        public int? Views { get; set; }

        public int? MajorId { get; set; }

        public bool? IsPrivate { get; set; }
    }
}
