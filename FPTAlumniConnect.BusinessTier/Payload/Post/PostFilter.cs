using FPTAlumniConnect.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.Post
{
    public class PostFilter
    {
        public int? AuthorId { get; set; }

        public string Content { get; set; } = null!;

        public string Title { get; set; } = null!;

        public int? Views { get; set; }

        public int? MajorId { get; set; }

        public string Status { get; set; } = null!;

        public bool? IsPrivate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
