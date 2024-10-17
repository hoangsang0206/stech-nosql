using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class UserAddress
{
    public int UDId { get; set; }

    public string RecipientName { get; set; } = null!;

    public string? RecipientPhone { get; set; }

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string WardCode { get; set; } = null!;

    public string District { get; set; } = null!;

    public string DistrictCode { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string ProvinceCode { get; set; } = null!;

    public bool? IsDefault { get; set; }

    public string AddressType { get; set; } = null!;
}
