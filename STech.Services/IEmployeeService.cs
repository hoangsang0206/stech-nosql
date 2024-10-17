using STech.Data.Models;

namespace STech.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeByUserId(string userId);
    }
}
