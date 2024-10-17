using MongoDB.Driver;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Brand> _collection;

        private readonly int BrandsPerPage = 20;

        public BrandService(StechDbContext context) => _collection = context.GetCollection<Brand>("Brands");

        public async Task<IEnumerable<Brand>> GetAll(bool isExcept)
        {
            if(isExcept)
            {
                return await _collection.Find(b => b.BrandId != "khac").SortBy(b => b.BrandName).ToListAsync();
            }

            return await _collection.Find(_ => true).SortBy(b => b.BrandName).ToListAsync();
        }

        public async Task<(IEnumerable<Brand>, int)> GetAll(string? sort_by, int page = 1)
        {
            IEnumerable<Brand> brands = await _collection.Find(_ => true).SortBy(b => b.BrandName).ToListAsync();
            int totalPages = (int)Math.Ceiling(brands.Count() / (double)BrandsPerPage);

            brands = brands.Sort(sort_by).Paginate(page, BrandsPerPage);

            return (brands, totalPages);
        }

        public async Task<Brand?> GetById(string id)
        {
            return await _collection.Find(b => b.BrandId == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Create(Brand brand)
        {
            try
            {
                await _collection.InsertOneAsync(brand);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Update(Brand brand)
        {
            try
            {
                ReplaceOneResult result = await _collection.ReplaceOneAsync(b => b.BrandId == brand.BrandId, brand);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                DeleteResult result = await _collection.DeleteOneAsync(b => b.BrandId == id);

                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch
            {
                return false;
            }          
        }
    }
}
