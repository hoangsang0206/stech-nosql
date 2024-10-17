using STech.Data.Models;
using STech.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IUserService
    {
        Task<User?> GetUser(LoginVM login);
        Task<User?> GetUserById(string id);
        Task<User?> GetUserByEmail(string email);
        Task<bool> IsExist(string username);
        Task<bool> IsEmailExist(string email);
        Task<bool> IsEmailExist(string userId, string email);
        Task<bool> CreateUser(RegisterVM register);
        Task<bool> CreateUser(ExternalRegisterVM register);
        Task<bool> UpdateUser(User user);
        Task<bool> UpdateAvatar(User user);
        Task<bool> ChangePassword(User user);

        Task<UserAddress?> GetUserMainAddress(string id);
        Task<IEnumerable<UserAddress>> GetUserAddress(string userId);
        Task<UserAddress?> GetUserAddress(string userId, int addressId);
        Task<bool> CreateUserAddress(string userId, UserAddress address);
        Task<bool> UpdateUserAddress(string userId, UserAddress address);
        Task<bool> SetDefaultAddress(string userId, int id);
        Task<bool> DeleteUserAddress(string userId, int id);
    }
}
