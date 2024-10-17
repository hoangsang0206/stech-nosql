using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IMongoCollection<DeliveryUnit> _collection;

        public FooterViewComponent(StechDbContext context) => _collection = context.GetCollection<DeliveryUnit>("DeliveryUnits");

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<DeliveryUnit> deliveryUnits = await _collection.Find(_ => true).ToListAsync();
            return View(deliveryUnits);
        }
    }
}
