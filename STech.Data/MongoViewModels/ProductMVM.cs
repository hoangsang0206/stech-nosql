using STech.Data.Models;

namespace STech.Data.MongoViewModels
{
    public class ProductMVM
    {
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

        public Brand? Brand { get; set; }

        public Category? Category { get; set; }

        public IEnumerable<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public IEnumerable<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();

        public IEnumerable<WarehouseProduct>? WarehouseProducts { get; set; }
    }
}
