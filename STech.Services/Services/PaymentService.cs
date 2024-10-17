using MongoDB.Driver;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IMongoCollection<PaymentMethod> _collection;

        public PaymentService(StechDbContext context)
        {
            _collection = context.GetCollection<PaymentMethod>("PaymentMethods");
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            return await _collection.Find(_ => true).SortBy(p => p.Sort).ToListAsync();
        }

        public async Task<PaymentMethod?> GetPaymentMethod(string id)
        {
            return await _collection.Find(p => p.PaymentMedId == id).FirstOrDefaultAsync();
        }
    }
}
