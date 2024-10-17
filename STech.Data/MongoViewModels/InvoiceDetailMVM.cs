using STech.Data.Models;

namespace STech.Data.MongoViewModels
{
    public class InvoiceDetailMVM
    {
        public string ProductId { get; set; } = null!;

        public decimal Cost { get; set; }

        public int Quantity { get; set; }

        public Product? Product { get; set; }
    }
}
