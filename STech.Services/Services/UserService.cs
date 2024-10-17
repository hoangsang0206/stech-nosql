using MongoDB.Bson;
using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Utils;
using System.Net;

namespace STech.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _collection;
        private readonly IMongoCollection<Role> _roles;

        public UserService(StechDbContext context)
        {
            _collection = context.GetCollection<User>("Users");
            _roles = context.GetCollection<Role>("Roles");
        }

        #region User

        public async Task<User?> GetUser(LoginVM login)
        {
            User? user = await _collection.Find(u => u.Username == login.UserName)
                .FirstOrDefaultAsync();

            if(user != null && user.PasswordHash == login.Password.HashPasswordMD5(user.RandomKey))
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetUserById(string id)
        {
            return await _collection.Find(u => u.UserId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _collection.Find(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsExist(string username)
        {
            User? user = await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<bool> IsEmailExist(string email)
        {
            User? user = await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<bool> IsEmailExist(string userId, string email)
        {
            User? user = await _collection.Find(u => u.Email == email && u.UserId != userId).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<bool> CreateUser(RegisterVM register)
        {
            try
            {
                if (register.RegPassword != register.ConfirmPassword) return false;

                string randomKey = UserUtils.GenerateRandomString(20);

                Role? role = await _roles.Find(r => r.RoleId == "user").FirstOrDefaultAsync();

                if (role == null || role.RoleId == null) return false;

                User user = new User()
                {
                    UserId = UserUtils.GenerateRandomId(40),
                    Username = register.RegUserName,
                    PasswordHash = register.RegPassword.HashPasswordMD5(randomKey),
                    Email = register.Email,
                    RandomKey = randomKey,
                    IsActive = true,
                    RoleId = role.RoleId,
                };

                await _collection.InsertOneAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateUser(ExternalRegisterVM register)
        {
            try
            {
                string randomKey = UserUtils.GenerateRandomString(20);

                Role? role = await _roles.Find(r => r.RoleId == "user").FirstOrDefaultAsync();

                if (role == null || role.RoleId == null)
                {
                    return false;
                }

                User user = new User()
                {
                    UserId = register.UserId,
                    Username = register.UserId ?? UserUtils.GenerateRandomId(30),
                    PasswordHash = UserUtils.GenerateRandomString(20).HashPasswordMD5(randomKey),
                    Email = register.Email,
                    EmailConfirmed = register.EmailConfirmed,
                    RandomKey = randomKey,
                    FullName = register.FullName,
                    Avatar = register.Avatar,
                    IsActive = true,
                    RoleId = role.RoleId,
                    AuthenticationProvider = register.AuthenticationProvider
                };

                await _collection.InsertOneAsync(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            UpdateDefinition<User> update = Builders<User>.Update
                .Set(u => u.FullName, user.FullName)
                .Set(u => u.Email, user.Email)
                .Set(u => u.Phone, user.Phone)
                .Set(u => u.Gender, user.Gender)
                .Set(u => u.Dob, user.Dob);

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == user.UserId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateAvatar(User user)
        {

            UpdateDefinition<User> update = Builders<User>.Update
                .Set(u => u.Avatar, user.Avatar);

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == user.UserId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ChangePassword(User user)
        {
            UpdateDefinition<User> update = Builders<User>.Update
                .Set(u => u.PasswordHash, user.PasswordHash);

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == user.UserId, update);
            return result.ModifiedCount > 0;
        }

        #endregion


        #region UserAddresses

        public async Task<UserAddress?> GetUserMainAddress(string id)
        {
            User? user = await GetUserById(id);

            return user?.UserAddresses.FirstOrDefault(u => u.IsDefault == true);
        }

        public async Task<IEnumerable<UserAddress>> GetUserAddress(string userId)
        {
            User? user = await GetUserById(userId);

            return user?.UserAddresses.OrderBy(d => !d.IsDefault).ToList() ?? new List<UserAddress>();
        }

        public async Task<UserAddress?> GetUserAddress(string userId, int addressId)
        {
            User? user = await GetUserById(userId);

            return user?.UserAddresses.FirstOrDefault(u => u.UDId == addressId);
        }

        public async Task<bool> CreateUserAddress(string userId, UserAddress address)
        {
            address.UDId = RandomUtils.RandomNumbers(1, 9999999);
            UpdateDefinition<User> update = Builders<User>.Update
                .Push(u => u.UserAddresses, address);

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == userId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateUserAddress(string userId, UserAddress address)
        {
            UpdateDefinition<User> update = Builders<User>.Update
                .Set("UserAddresses.$[elem].RecipientName", address.RecipientName)
                .Set("UserAddresses.$[elem].RecipientPhone", address.RecipientPhone)
                .Set("UserAddresses.$[elem].Address", address.Address)
                .Set("UserAddresses.$[elem].Province", address.Province)
                .Set("UserAddresses.$[elem].ProvinceCode", address.ProvinceCode)
                .Set("UserAddresses.$[elem].District", address.District)
                .Set("UserAddresses.$[elem].DistrictCode", address.DistrictCode)
                .Set("UserAddresses.$[elem].Ward", address.Ward)
                .Set("UserAddresses.$[elem].WardCode", address.WardCode)
                .Set("UserAddresses.$[elem].AddressType", address.AddressType);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("elem.UDId", address.UDId))
            };

            UpdateOptions options = new UpdateOptions()
            {
                ArrayFilters = arrayFilter
            };

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == userId, update, options);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> SetDefaultAddress(string userId, int id)
        {
            UpdateDefinition<User> update_all_false = Builders<User>.Update
               .Set("UserAddresses.$[].IsDefault", false);

            await _collection.UpdateManyAsync(u => u.UserId == userId, update_all_false);

            UpdateDefinition<User> update = Builders<User>.Update
               .Set("UserAddresses.$[elem].IsDefault", true);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("elem.UDId", id))
            };

            UpdateOptions options = new UpdateOptions()
            {
                ArrayFilters = arrayFilter
            };

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == userId, update, options);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAddress(string userId, int id)
        {
            UpdateDefinition<User> update = Builders<User>.Update.PullFilter(
               u => u.UserAddresses,
               address => address.UDId == id
            );

            UpdateResult result = await _collection.UpdateOneAsync(u => u.UserId == userId, update);
            return result.ModifiedCount > 0;
        }

        #endregion
    }
}
