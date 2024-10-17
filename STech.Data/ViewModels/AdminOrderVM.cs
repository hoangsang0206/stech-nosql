using System.ComponentModel.DataAnnotations;
namespace STech.Data.ViewModels
{
    public class AdminOrderVM
    {
        public string? Address { get; set; }

        public string? WardCode { get; set; }

        public string? DistrictCode { get; set; }

        public string? CityCode { get; set; }

        public string? RecipientName { get; set; }

        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$")]
        public string? RecipientPhone { get; set; }

        [Required(ErrorMessage = "* Chọn cách nhận hàng")]
        public string DeliveryMethod { get; set; } = null!;

        public string? Note { get; set; }

        [Required]
        public string WarehouseId { get; set; } = null!;

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required(ErrorMessage = "* Chọn sản phẩm")]
        public List<Product> Products { get; set; } = new List<Product>();

        public class Product
        {
            public string ProductId { get; set; } = null!;
            public int Quantity { get; set; }
            public decimal? SalePrice { get; set; }
        }
    }
}
