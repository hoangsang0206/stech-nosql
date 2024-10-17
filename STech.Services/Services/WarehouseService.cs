using MongoDB.Driver;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IMongoCollection<Warehouse> _collection;
        private readonly IMongoCollection<WarehouseExport> _wheCollection;

        private readonly Lazy<IProductService> _productService;

        public WarehouseService(StechDbContext context, Lazy<IProductService> productService)
        {
            _collection = context.GetCollection<Warehouse>("Warehouses");
            _wheCollection = context.GetCollection<WarehouseExport>("WarehouseExports");
            _productService = productService;
        }

        public async Task<IEnumerable<Warehouse>> GetWarehouses()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistance(double latitude, double longtitude)
        {
            IEnumerable<Warehouse> warehouses = await GetWarehouses();
            return warehouses.OrderByDistance(latitude, longtitude);
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistanceWithProduct(double? latitude, double? longtitude)
        {
            IEnumerable<Warehouse> warehouses = warehouses = await GetWarehouses();

            if (latitude == null || longtitude == null)
            {
                return warehouses.OrderByDescending(w => _productService.Value.GetAllWarehouseProducts(w.WarehouseId).Result.Count());
            }

            return warehouses.OrderByDistance(latitude.Value, longtitude.Value);
        }

        public async Task<Warehouse?> GetNearestWarehouse(double latitude, double longtitude)
        {
            IEnumerable<Warehouse> warehouses = await GetWarehousesOrderByDistance(latitude, longtitude);
            Warehouse? warehouse = warehouses.FirstOrDefault();

            return warehouse;
        }

        public async Task<Warehouse?> GetWarehouseById(string warehouseId)
        {
            return await _collection.Find(w => w.WarehouseId == warehouseId).FirstOrDefaultAsync();
        }

        

        public async Task<bool> CreateWarehouseExports(IEnumerable<WarehouseExport> warehouseExports)
        {
            try
            {
                await _wheCollection.InsertManyAsync(warehouseExports);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
