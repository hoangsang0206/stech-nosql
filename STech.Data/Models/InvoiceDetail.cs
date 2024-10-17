using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class InvoiceDetail
{
    public string ProductId { get; set; } = null!;

    public decimal Cost { get; set; }

    public int Quantity { get; set; }

    public string? SaleId { get; set; }
}
