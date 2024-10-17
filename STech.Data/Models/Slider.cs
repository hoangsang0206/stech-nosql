using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Slider
{
    public ObjectId _id { get; set; }

    public int SId { get; set; }

    public string SliderImgSrc { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;
}
