using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Brand
{
    public ObjectId _id { get; set; }

    public string BrandId { get; set; } = null!;

    public string BrandName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? LogoSrc { get; set; }

}
