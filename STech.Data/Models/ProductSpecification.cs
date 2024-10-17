using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductSpecification
{
    public int SpecId { get; set; }

    public string SpecName { get; set; } = null!;

    public string SpecValue { get; set; } = null!;
}
