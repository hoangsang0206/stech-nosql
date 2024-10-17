using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductImage
{
    public int ImgId { get; set; }

    public string ImageSrc { get; set; } = null!;
}
