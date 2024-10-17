using MongoDB.Driver;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMongoCollection<Employee> _collection;
        private readonly IMongoCollection<User> _userCollection;

        public EmployeeService(StechDbContext context)
        {
            _collection = context.GetCollection<Employee>("Employees");
            _userCollection = context.GetCollection<User>("Users");
        }

        public async Task<Employee?> GetEmployeeByUserId(string userId)
        {
            User? user = await _userCollection
                .Find(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            return await _collection.Find(e => e.EmployeeId == user.EmployeeId).FirstOrDefaultAsync();
        }
    }
}
