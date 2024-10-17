using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Warehouse
{
    public ObjectId _id { get; set; }

    public string WarehouseId { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string WardCode { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DistrictCode { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string ProvinceCode { get; set; } = null!;

    public string? Type { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longtitude { get; set; }
}
