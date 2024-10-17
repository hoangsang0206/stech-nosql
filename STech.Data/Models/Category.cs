using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Category
{
    public ObjectId _id { get; set; }

    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string? ImageSrc { get; set; }
}
