using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ReviewLike
{
    public int RLId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime LikeDate { get; set; }

}
