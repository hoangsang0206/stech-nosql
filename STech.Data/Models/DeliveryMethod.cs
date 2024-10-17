using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class DeliveryMethod
{
    public ObjectId _id { get; set; }

    public string DeliveryMedId { get; set; } = null!;

    public string DeliveryName { get; set; } = null!;

}
