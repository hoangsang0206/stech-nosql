using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class SubHeader
{
    public ObjectId _id { get; set; }

    public int SHId { get; set; }

    public string? Icon { get; set; }

    public string Title { get; set; } = null!;

    public string? RedirectUrl { get; set; }
}
