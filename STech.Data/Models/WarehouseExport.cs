using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseExport
{
    public ObjectId _id { get; set; }

    public string Weid { get; set; } = null!;

    public DateTime DateCreate { get; set; }

    public DateTime? DateExport { get; set; }

    public string? ReasonExport { get; set; }

    public string? Note { get; set; }

    public string? InvoiceId { get; set; }

    public string? EmployeeId { get; set; }

    public string WarehouseId { get; set; } = null!;

    public List<WarehouseExportDetail> WarehouseExportDetails { get; set; } = new List<WarehouseExportDetail>();
}
