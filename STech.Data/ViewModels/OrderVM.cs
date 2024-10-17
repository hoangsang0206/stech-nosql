using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class OrderVM
    {
        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string WardCode { get; set; } = null!;

        [Required]
        public string DistrictCode { get; set; } = null!;

        [Required]
        public string CityCode { get; set; } = null!;

        [Required]
        public string RecipientName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$")]
        public string RecipientPhone { get; set; } = null!;

        [Required]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        public string DeliveryMethod { get; set; } = null!;

        public string? Note { get; set; }

        public string? pId { get; set; }
    }
}
