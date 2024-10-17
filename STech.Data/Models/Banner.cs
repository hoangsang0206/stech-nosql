using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Banner
{
    [BsonIgnore]
    public ObjectId _id { get; set; }

    public int BId { get; set; }

    public string BannerImgSrc { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public int? BannerTypeId { get; set; }

}
