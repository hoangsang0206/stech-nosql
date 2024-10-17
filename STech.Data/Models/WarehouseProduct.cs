using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseProduct
{
    public string WarehouseId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }
}
