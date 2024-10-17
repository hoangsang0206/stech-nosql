using STech.Data.Models;

namespace STech.Data.MongoViewModels
{
    public class CartMVM
    {
        public int Id { get; set; }

        public string ProductId { get; set; } = null!;

        public int Quantity { get; set; }

        public ProductMVM Product { get; set; } = null!;
    }
}
