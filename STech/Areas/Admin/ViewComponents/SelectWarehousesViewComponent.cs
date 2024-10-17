using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Services;

namespace STech.Areas.Admin.ViewComponents
{
    public class SelectWarehousesViewComponent : ViewComponent
    {
        private readonly IWarehouseService _warehouseService;

        public SelectWarehousesViewComponent(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();

            return View(warehouses);
        }
    }
}
