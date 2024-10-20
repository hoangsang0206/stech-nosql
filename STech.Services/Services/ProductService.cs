using MongoDB.Bson;
using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _collection;

        private readonly Lazy<IBrandService> _brandService;
        private readonly Lazy<ICategoryService> _categoryService;

        private readonly int NumOfProductPerPage = 40;

        public ProductService(StechDbContext context, Lazy<IBrandService> brandService, Lazy<ICategoryService> categoryService)
        {
            _collection = context.GetCollection<Product>("Products");
            _brandService = brandService;
            _categoryService = categoryService;
        }


        #region GET
        public async Task<(IEnumerable<Product>, int)> GetProducts(string? brands, string? categories, string? status, string? price_range, string? warehouse_id, string? sort, int page = 1)
        {
            IEnumerable<Product> products = await _collection
                .Find(p => warehouse_id == null
                    ||  p.WarehouseProducts.Where(wp => wp.WarehouseId == warehouse_id).Count() > 0).ToListAsync();

            products = products.SelectProduct(warehouse_id)
                .Filter(brands, categories, status, price_range)
                .Sort(sort);

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));



            return (products.Pagnigate(page, NumOfProductPerPage), totalPage);
        }



        public async Task<(IEnumerable<Product>, int)> SearchByName(string q, int page, string? sort)
        {
            if(string.IsNullOrEmpty(q))
            {
                return (new List<Product>(), 1);
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            FilterDefinition<Product> filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.IsActive, true),
                Builders<Product>.Filter.And(keywords.Select(key =>
                    Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(key, "i")))
                )
            );

            IEnumerable<Product> products = await _collection
                .Find(filter)
                .ToListAsync();

            products = products.Sort(sort).SelectProduct();

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products.Pagnigate(page, NumOfProductPerPage), totalPage);
        }

        public async Task<(IEnumerable<Product>, int)> SearchProducts(string q, int page, string? sort, string? warehouseId)
        {
            if (string.IsNullOrEmpty(q))
            {
                return (new List<Product>(), 1);
            }

            string[] keywords = q.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            FilterDefinition<Product> filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(p => p.ProductId, q),
                Builders<Product>.Filter.And(
                    Builders<Product>.Filter.Eq(p => p.IsActive, true),
                    Builders<Product>.Filter.And(keywords.Select(key =>
                    Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(key, "i")))
                ))
            );

            IEnumerable<Product> products = await _collection
                .Find(filter)
                .ToListAsync();

            products = products.Where(p => warehouseId != null ? p.WarehouseProducts.Any(w => w.WarehouseId == warehouseId) : true).ToList();

            products = products.SelectProduct(warehouseId);

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products.Sort(sort).Pagnigate(page, NumOfProductPerPage), totalPage);
        }

        public async Task<(IEnumerable<Product>, int)> GetByCategory(string categoryId, int page, string? sort)
        {
            IEnumerable<Product> products = await _collection
                .Find(p => p.CategoryId == categoryId && p.IsActive == true)
                .ToListAsync();

            products = products.SelectProduct();

            int totalPage = Convert.ToInt32(Math.Ceiling(
                Convert.ToDouble(products.Count()) / Convert.ToDouble(NumOfProductPerPage)));

            return (products.Sort(sort).Pagnigate(page, NumOfProductPerPage), totalPage);
        }

        public async Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake)
        {
            IEnumerable<Product> products = await _collection
                .Find(p => p.CategoryId == categoryId && p.IsActive == true && p.WarehouseProducts.Sum(wp => wp.Quantity) > 0)
                .ToListAsync();

            return products.Take(numToTake).OrderBy(p => Guid.NewGuid()).SelectProduct();
        }

        public async Task<ProductMVM?> GetProduct(string id)
        {
            Product product = await _collection
                .Find(p => p.ProductId == id)
                .FirstOrDefaultAsync();

            return new ProductMVM
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                ProductImages = product.ProductImages,
                WarehouseProducts = product.WarehouseProducts,
                BrandId = product.BrandId,
                Brand = await _brandService.Value.GetById(product.BrandId ?? ""),
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                ProductSpecifications = product.ProductSpecifications,
                CategoryId = product.CategoryId,
                Category = await _categoryService.Value.GetOne(product.CategoryId ?? ""),
                ManufacturedYear = product.ManufacturedYear,
                Warranty = product.Warranty,
                IsActive = product.IsActive,
                IsDeleted = product.IsDeleted,
            };
        }

        public async Task<ProductMVM?> GetProductWithBasicInfo(string id)
        {
            Product p = await _collection
                .Find(p => p.ProductId == id && p.IsActive == true)
                .FirstOrDefaultAsync();

            return new ProductMVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.ImgId).Take(1).ToList(),
                WarehouseProducts = p.WarehouseProducts,
                Brand = await _brandService.Value.GetById(p.BrandId ?? "")
            };
        }

        public async Task<ProductMVM?> GetProductWithBasicInfo(string id, string warehouseId)
        {
            Product p =await _collection
                .Find(p => p.ProductId == id && p.IsActive == true 
                        && p.WarehouseProducts.Any(w => w.WarehouseId == warehouseId))
                .FirstOrDefaultAsync();

            return new ProductMVM
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.ImgId).Take(1).ToList(),
                WarehouseProducts = p.WarehouseProducts,
                Brand = await _brandService.Value.GetById(p.BrandId ?? ""),
                Category = await _categoryService.Value.GetOne(p.CategoryId ?? ""),
            };
        }

        public async Task<bool> CheckOutOfStock(string id)
        {
            Product product = await _collection
                .Find(p => p.ProductId == id && p.IsActive == true)
                .FirstOrDefaultAsync() 
                ?? new Product();

            int totalQty = product.WarehouseProducts.Sum(p => p.Quantity);

            return totalQty <= 0;
        }

        public async Task<int> GetTotalQty(string id)
        {
            Product product = await _collection
                .Find(p => p.ProductId == id && p.IsActive == true)
                .FirstOrDefaultAsync()
                ?? new Product();

            return product.WarehouseProducts
                .Sum(p => p.Quantity);
        }

        public async Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId)
        {
            Product p = await _collection.Find(p => p.ProductId == productId)
                .FirstOrDefaultAsync();

            return p.WarehouseProducts;
        }

        public async Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId, string warehouseId)
        {
            Product p = await _collection.Find(p => p.ProductId == productId)
                .FirstOrDefaultAsync();

            return p.WarehouseProducts.Where(wp => wp.WarehouseId == warehouseId).ToList();
        }

        public async Task<IEnumerable<WarehouseProduct>> GetAllWarehouseProducts(string warehouseId)
        {
            IEnumerable<Product> products = await _collection
                .Find(p => p.WarehouseProducts.Any(wp => wp.WarehouseId == warehouseId))
                .ToListAsync();

            return products.SelectMany(p => p.WarehouseProducts.Where(wp => wp.WarehouseId == warehouseId)).ToList();
        }

        #endregion GET


        #region POST

        public async Task<bool> CreateProduct(ProductVM product)
        {
            try
            {
                Product _product = new Product()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName.Trim(),
                    Price = product.Price,
                    OriginalPrice = product.OriginalPrice,
                    Warranty = product.Warranty,
                    ManufacturedYear = product.ManufacturedYear,
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    ShortDescription = product.ShortDescription,
                    Description = product.Description,
                    IsActive = false,
                    IsDeleted = false,
                    DateAdded = DateTime.Now,
                };

                if (product.Specifications != null && product.Specifications.Count > 0 && !product.Specifications.Any(s => s == null))
                {
                    foreach (ProductVM.Specification spec in product.Specifications)
                    {
                        _product.ProductSpecifications.Add(new ProductSpecification
                        {
                            SpecName = spec.Name,
                            SpecValue = spec.Value
                        });
                    }
                }

                if (product.Images != null && product.Images.Count > 0 && !product.Images.Any(i => i == null))
                {
                    foreach (ProductVM.Image image in product.Images)
                    {
                        _product.ProductImages.Add(new ProductImage
                        {
                            ImageSrc = image.ImageSrc
                        });
                    }
                }

                await _collection.InsertOneAsync(_product);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion POST


        #region PUT

        public async Task<bool> UpdateProduct(ProductVM product)
        {
            Product? _product = await _collection.Find(p => p.ProductId == product.ProductId).FirstOrDefaultAsync();
            if (_product == null)
            {
                return false;
            }
            
            if (product.Specifications != null && product.Specifications.Count > 0 && !product.Specifications.Any(s => s == null))
            {
                foreach (ProductVM.Specification spec in product.Specifications)
                {
                    _product.ProductSpecifications.Add(new ProductSpecification
                    {
                        SpecName = spec.Name,
                        SpecValue = spec.Value
                    });
                }
            }

            if(product.Images != null && product.Images.Count > 0 && !product.Images.Any(i => i == null))
            {
                foreach(ProductVM.Image image in product.Images)
                {
                    if(image.Id != null)
                    {
                        if (image.Status == "deleted")
                        {
                            ProductImage? pImage = _product.ProductImages.Where(i => i.ImgId == image.Id).FirstOrDefault();
                            if (pImage != null)
                            {
                                _product.ProductImages.Remove(pImage);
                            }
                        }
                    } 
                    else
                    {
                        _product.ProductImages.Add(new ProductImage
                        {
                            ImgId = RandomUtils.RandomNumbers(1, 99999999),
                            ImageSrc = image.ImageSrc
                        });
                    }
                }
            }

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.ProductName, product.ProductName.Trim())
                .Set(p => p.Price, product.Price)
                .Set(p => p.OriginalPrice, product.OriginalPrice)
                .Set(p => p.Warranty, product.Warranty)
                .Set(p => p.ManufacturedYear, product.ManufacturedYear)
                .Set(p => p.BrandId, product.BrandId)
                .Set(p => p.CategoryId, product.CategoryId)
                .Set(p => p.ShortDescription, product.ShortDescription)
                .Set(p => p.Description, product.Description)
                .Set(p => p.ProductSpecifications, _product.ProductSpecifications)
                .Set(p => p.ProductImages, _product.ProductImages);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == product.ProductId, update);

            return result.MatchedCount > 0;
        }

        public async Task<bool> SubtractProductQuantity(IEnumerable<WarehouseExport> warehouseExports)
        {
            long modifiedCount = 0;

            foreach (WarehouseExport wE in warehouseExports)
            {
                foreach (WarehouseExportDetail detail in wE.WarehouseExportDetails)
                {
                    FilterDefinition<Product> filter = Builders<Product>.Filter.And(
                        Builders<Product>.Filter.Eq(p => p.ProductId, detail.ProductId),
                        Builders<Product>.Filter.ElemMatch(p => p.WarehouseProducts, wp => wp.WarehouseId == wE.WarehouseId)
                    );

                    UpdateDefinition<Product> update = Builders<Product>.Update.Inc("WarehouseProducts.$.Quantity", -detail.RequestedQuantity);

                    UpdateResult result = await _collection.UpdateOneAsync(filter, update);
                    modifiedCount += result.ModifiedCount;
                }
            }

            return modifiedCount > 0;
        }

        #endregion PUT


        #region DELETE

        public async Task<bool> DeleteProduct(string id)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsActive, false)
                .Set(p => p.IsDeleted, true)
                .Set(p => p.DateDeleted, DateTime.Now);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProducts(string[] ids)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
               .Set(p => p.IsActive, false)
               .Set(p => p.IsDeleted, true)
               .Set(p => p.DateDeleted, DateTime.Now);

            UpdateResult result = await _collection.UpdateManyAsync(p => ids.Contains(p.ProductId), update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> PermanentlyDeleteProduct(string id)
        {
            DeleteResult result = await _collection.DeleteOneAsync(p => p.ProductId == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> PermanentlyDeleteProducts(string[] ids)
        {
            DeleteResult result = await _collection.DeleteManyAsync(p => ids.Contains(p.ProductId));
            return result.DeletedCount > 0;
        }

        #endregion DELETE


        #region RESTORE

        public async Task<bool> RestoreProduct(string id)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsDeleted, false)
                .Set(p => p.DateDeleted, null);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RestoreProducts(string[] ids)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsDeleted, false)
                .Set(p => p.DateDeleted, null);

            UpdateResult result = await _collection.UpdateManyAsync(p => ids.Contains(p.ProductId), update);
            return result.ModifiedCount > 0;
        }

        #endregion RESTORE

        #region ACTIVATE

        public async Task<bool> ActivateProduct(string id)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsActive, true);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == id, update);
            return result.ModifiedCount > 0;
        }
        public async Task<bool> ActivateProducts(string[] ids)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsActive, true);

            UpdateResult result = await _collection.UpdateManyAsync(p => ids.Contains(p.ProductId), update);
            return result.ModifiedCount > 0;
        }

        #endregion ACTIVATE

        #region DEACTIVATE

        public async Task<bool> DeActivateProduct(string id)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsActive, false);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeActivateProducts(string[] ids)
        {

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set(p => p.IsActive, false);

            UpdateResult result = await _collection.UpdateManyAsync(p => ids.Contains(p.ProductId), update);
            return result.ModifiedCount > 0;
        }

        #endregion DEACTIVATE
    }
}
