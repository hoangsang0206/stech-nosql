using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class PaymentMethod
{
    public ObjectId _id { get; set; }

    public string PaymentMedId { get; set; } = null!;

    public string PaymentName { get; set; } = null!;

    public string? LogoSrc { get; set; }

    public bool? IsActive { get; set; }

    public int Sort { get; set; }
}
