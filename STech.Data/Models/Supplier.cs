using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Supplier
{
    public ObjectId _id { get; set; }

    public string SupplierId { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;
}
