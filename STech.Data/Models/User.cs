using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class User
{
    public ObjectId _id { get; set; }

    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    [JsonIgnore]
    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string? Phone { get; set; }

    public bool? PhoneConfirmed { get; set; }

    public string? Avatar { get; set; }

    [JsonIgnore]
    public string RandomKey { get; set; } = null!;

    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public bool? IsActive { get; set; }

    public string RoleId { get; set; } = null!;

    public string? AuthenticationProvider { get; set; }

    public string? EmployeeId { get; set; }

    [JsonIgnore]
    public List<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
