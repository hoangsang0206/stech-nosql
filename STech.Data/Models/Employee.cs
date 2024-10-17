using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Employee
{
    public ObjectId _id { get; set; }

    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string CitizenId { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string WardCode { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DistrictCode { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string ProvinceCode { get; set; } = null!;
}
