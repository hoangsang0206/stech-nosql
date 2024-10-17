using MongoDB.Driver;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<Customer> _collection;

        public CustomerService(StechDbContext context) {
            _collection = context.GetCollection<Customer>("Customers");
        }

        public async Task<IEnumerable<Customer>> SearchCustomers(string phone)
        {
            return await _collection
                .Find(c => c.Phone.Contains(phone))
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerById(string id)
        {
            return await _collection.Find(c => c.CustomerId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            try
            {
                customer.CustomerId = DateTime.Now.ToString("yyyyMMdd") + UserUtils.GenerateRandomString(8).ToUpper();

                await _collection.InsertOneAsync(customer);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
