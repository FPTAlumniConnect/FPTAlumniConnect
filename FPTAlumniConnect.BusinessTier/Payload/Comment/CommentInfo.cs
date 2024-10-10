using System;
using System.Collections.Generic;

namespace FPTAlumniConnect.BusinessTier.Payload.Comment;

public class CommentInfo
{
    public string Content { get; set; } = null!;

    public string? Type { get; set; }
}
