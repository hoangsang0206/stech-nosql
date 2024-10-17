using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> SearchCustomers(string phone);
        Task<Customer?> GetCustomerById(string id);
        Task<bool> CreateCustomer(Customer customer);
    }
}
