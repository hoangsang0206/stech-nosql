using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class UserCart
{
    public ObjectId _id { get; set; }

    public int CId { get; set; }

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public string UserId { get; set; } = null!;
}
