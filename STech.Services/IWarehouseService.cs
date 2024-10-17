using STech.Data.Models;
using STech.Data.MongoViewModels;

namespace STech.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistance(double latitude, double longtitude);
        Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistanceWithProduct(double? latitude, double? longtitude);
        Task<Warehouse?> GetNearestWarehouse(double latitude, double longtitude);
        Task<Warehouse?> GetWarehouseById(string warehouseId);
        Task<bool> CreateWarehouseExports(IEnumerable<WarehouseExport> warehouseExports);
    }
}
