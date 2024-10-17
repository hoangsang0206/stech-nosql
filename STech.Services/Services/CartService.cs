using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class CartService : ICartService
    {
        private readonly IMongoCollection<UserCart> _collection;
        private readonly IMongoCollection<Product> _productCollection;
        
        public CartService(StechDbContext context)
        {
            _collection = context.GetCollection<UserCart>("UserCarts");
            _productCollection = context.GetCollection<Product>("Products");
        }

        public async Task<bool> AddToCart(UserCart cart)
        {
            try
            {
                if (cart == null || cart.UserId == null || cart.ProductId == null || cart.Quantity <= 0)
                {
                    return false;
                }

                cart.CId = RandomUtils.RandomNumbers(1, 999999999);
                
                await _collection.InsertOneAsync(cart);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<UserCart?> GetUserCartItem(string userId, string productId)
        {
            if (userId == null || productId == null)
            {
                return new UserCart();
            }

            return await _collection.Find(c => c.UserId == userId && c.ProductId == productId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CartMVM>> GetUserCart(string userId)
        {
            if(userId == null)
            {
                return new List<CartMVM>();
            }

            IEnumerable<UserCart> cart = await _collection.Find(c => c.UserId == userId).ToListAsync();

            List<CartMVM> data = new List<CartMVM>();

            foreach(UserCart c in cart)
            {
                Product? product = await _productCollection.Find(p => p.ProductId == c.ProductId).FirstOrDefaultAsync();
                if (product == null)
                {
                    continue;
                }

                data.Add(new CartMVM
                {
                    Id = c.CId,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Product = new ProductMVM
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        OriginalPrice = product.OriginalPrice,
                        ProductImages = product.ProductImages.OrderBy(i => i.ImgId).Take(1).ToList(),
                    }
                });
            }

            return data;
        }

        public async Task<bool> RemoveFromCart(UserCart cart)
        {
            try
            {
                if (cart == null)
                {
                    return false;
                }

                DeleteResult result = await _collection.DeleteOneAsync(c => c.CId == cart.CId);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveUserCart(string userId)
        {
            try
            {
                if (userId == null)
                {
                    return false;
                }

                DeleteResult result = await _collection.DeleteManyAsync(c => c.UserId == userId);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }         
        }

        public async Task<bool> RemoveListCart(IEnumerable<CartMVM> cart)
        {
            try
            {
                if (cart.Count() <= 0)
                {
                    return false;
                }

                int[] ids = cart.Select(c => c.Id).ToArray();
                FilterDefinition<UserCart> filter = Builders<UserCart>.Filter.In(c => c.CId, ids);

                DeleteResult result = await _collection.DeleteManyAsync(filter);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }           
        }

        public async Task<bool> UpdateQuantity(UserCart cart, int qty)
        {
            try
            {
                FilterDefinition<UserCart> filter = Builders<UserCart>.Filter.Eq(c => c.CId, cart.CId);
                UpdateDefinition<UserCart> update = Builders<UserCart>.Update.Set(c => c.Quantity, qty);

                UpdateResult result = await _collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
