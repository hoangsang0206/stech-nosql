using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _collection;
        private readonly IMongoCollection<Product> _pCollection;
        private readonly IMongoCollection<Brand> _bCollection;

        private readonly int CategoriesPerPage = 30;

        public CategoryService(StechDbContext context)
        {
            _collection = context.GetCollection<Category>("Categories");
            _pCollection = context.GetCollection<Product>("Products");
            _bCollection = context.GetCollection<Brand>("Brands");
        }

        public async Task<IEnumerable<Category>> GetAll(bool isExcept)
        {
            if(isExcept)
            {
                return await _collection.Find(c => c.CategoryId != "khac").SortBy(c => c.CategoryName).ToListAsync();
            }

            return await _collection.Find(_ => true).SortBy(c => c.CategoryName).ToListAsync();
        }

        public async Task<IEnumerable<CategoryMVM>> GetRandomWithProducts(int numCategories, int numProducts)
        {
            if(numCategories == 0 || numProducts == 0) { return Enumerable.Empty<CategoryMVM>(); }

            IEnumerable<Category> categories = await _collection
                .Find(c => c.CategoryId != "khac")
                .ToListAsync();

            List<CategoryMVM> _categories = new List<CategoryMVM>();

            foreach(Category category in categories.OrderBy(c => Guid.NewGuid()))
            {
                IEnumerable<Product> products = await _pCollection
                    .Find(p => p.CategoryId == category.CategoryId && p.IsActive == true)
                    .ToListAsync();

                _categories.Add(new CategoryMVM
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Products = products
                        .Select(p => new ProductMVM
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                            ProductImages = p.ProductImages.OrderBy(pp => pp.ImgId).Take(1).ToList(),
                            WarehouseProducts = _pCollection.Find(pp => pp.ProductId == p.ProductId)
                                .FirstOrDefaultAsync().Result.WarehouseProducts,
                            Brand = _bCollection.Find(b => b.BrandId == p.BrandId).FirstOrDefaultAsync().Result
                        })
                        .Take(numProducts).ToList()
                });
            }

            return _categories.Where(c => c.Products?.Count() > 10).Take(numCategories).ToList();
        }

        public async Task<(IEnumerable<CategoryMVM>, int)> GetAllWithProducts(string? sort_by, int page = 1)
        {
            IEnumerable<Category> categories = await _collection.Find(_ => true)
                .SortBy(c => c.CategoryName)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling(categories.Count() / (double)CategoriesPerPage);

            categories = categories.Sort(sort_by).Paginate(page, CategoriesPerPage);

            List<CategoryMVM> _categories = new List<CategoryMVM>();

            foreach (Category category in categories)
            {
                IEnumerable<Product> products= await _pCollection.Find(p => p.CategoryId == category.CategoryId).ToListAsync();

                _categories.Add(new CategoryMVM
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ImageSrc = category.ImageSrc,
                    Products = products
                        .Select(p => new ProductMVM
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            OriginalPrice = p.OriginalPrice,
                            Price = p.Price,
                        })
                });
            }

            return (_categories, totalPages);
        }

        public async Task<Category?> GetOne(string id)
        {
            return await _collection.Find(c => c.CategoryId == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Create(Category category)
        {
            try
            {
                await _collection.InsertOneAsync(category);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<bool> Update(Category category)
        {
            ReplaceOneResult result = await _collection.ReplaceOneAsync(c => c.CategoryId == category.CategoryId, category);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            DeleteResult result = await _collection.DeleteOneAsync(c => c.CategoryId == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
