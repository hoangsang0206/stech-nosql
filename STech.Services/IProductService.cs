using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product>, int)> GetProducts(string? brands, string? categories, string? status, string? price_range, string? warehouse_id, string? sort, int page = 1);
        Task<(IEnumerable<Product>, int)> SearchByName(string q, int page, string? sort);
        Task<(IEnumerable<Product>, int)> SearchProducts(string q, int page, string? sort, string? warehouseId);
        Task<(IEnumerable<Product>, int)> GetByCategory(string categoryId, int page, string? sort);
        Task<IEnumerable<Product>> GetSimilarProducts(string categoryId, int numToTake);
        Task<ProductMVM?> GetProduct(string id);
        Task<ProductMVM?> GetProductWithBasicInfo(string id);
        Task<ProductMVM?> GetProductWithBasicInfo(string id, string warehouseId);
        Task<bool> CheckOutOfStock(string id);
        Task<int> GetTotalQty(string id);
        Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId);
        Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId, string warehouseId);
        Task<IEnumerable<WarehouseProduct>> GetAllWarehouseProducts(string warehouseId);

        Task<bool> DeleteProduct(string id);
        Task<bool> DeleteProducts(string[] ids);
        Task<bool> PermanentlyDeleteProduct(string id);
        Task<bool> PermanentlyDeleteProducts(string[] ids);

        Task<bool> RestoreProduct(string id);
        Task<bool> RestoreProducts(string[] ids);

        Task<bool> ActivateProduct(string id);
        Task<bool> ActivateProducts(string[] ids);

        Task<bool> DeActivateProduct(string id);
        Task<bool> DeActivateProducts(string[] ids);

        Task<bool> UpdateProduct(ProductVM product);
        Task<bool> CreateProduct(ProductVM product);
        Task<bool> SubtractProductQuantity(IEnumerable<WarehouseExport> warehouseExports);
    }
}
