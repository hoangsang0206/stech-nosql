using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class BannerType
{
    public ObjectId _id { get; set; }

    public int BId { get; set; }

    public string Type { get; set; } = null!;

}
