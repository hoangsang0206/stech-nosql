using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Role
{
    public ObjectId _id { get; set; }

    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;
}
