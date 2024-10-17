using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ReviewReply
{
    public int RRId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime ReplyDate { get; set; }

    public bool? IsRead { get; set; }

    public string UserReplyId { get; set; } = null!;
}
