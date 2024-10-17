using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class DeliveryUnit
{
    public ObjectId _id { get; set; }

    public string Duid { get; set; } = null!;

    public string Duname { get; set; } = null!;

    public string? Phone { get; set; }

    public string? LogoSrc { get; set; }

}
