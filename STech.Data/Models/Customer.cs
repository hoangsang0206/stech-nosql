using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Customer
{
    public ObjectId _id { get; set; }

    public string CustomerId { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string WardCode { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DistrictCode { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string ProvinceCode { get; set; } = null!;

}
