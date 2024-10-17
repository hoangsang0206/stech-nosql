using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Product
{
    public ObjectId _id { get; set; }

    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public int? ManufacturedYear { get; set; }

    public decimal? OriginalPrice { get; set; }

    public decimal Price { get; set; }

    public int? Warranty { get; set; }

    public string? CategoryId { get; set; }

    public string? BrandId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DateAdded { get; set; }

    public DateTime? DateDeleted { get; set; }

    public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public List<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();

    [JsonIgnore]
    public List<Review> Reviews { get; set; } = new List<Review>();

    public List<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();
}
