using MongoDB.Driver;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IMongoCollection<DeliveryMethod> _collection;

        public DeliveryService(StechDbContext context)
        {
            _collection = context.GetCollection<DeliveryMethod>("DeliveryMethods");
        }

        public async Task<DeliveryMethod?> GetDeliveryMethodById(string id)
        {
            return await _collection.Find(d => d.DeliveryMedId == id).FirstOrDefaultAsync();
        }
    }
}
