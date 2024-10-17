using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class InvoiceStatus
{
    public int IsId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? DateUpdated { get; set; }
}
