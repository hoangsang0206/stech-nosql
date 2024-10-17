using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ReviewImage
{
    public int RImgId { get; set; }

    public string ImageUrl { get; set; } = null!;
}
