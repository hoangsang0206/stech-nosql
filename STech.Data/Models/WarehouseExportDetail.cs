using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseExportDetail
{
    public int WEdId { get; set; }

    public string ProductId { get; set; } = null!;

    public int RequestedQuantity { get; set; }

    public int ActualQuantity { get; set; }

    public decimal UnitPrice { get; set; }
}
