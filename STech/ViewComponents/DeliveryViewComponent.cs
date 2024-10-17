using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class DeliveryViewComponent : ViewComponent
    {
        private readonly IMongoCollection<DeliveryMethod> _collection;

        public DeliveryViewComponent(StechDbContext context)
        {
            _collection = context.GetCollection<DeliveryMethod>("DeliveryMethods");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<DeliveryMethod> methods = await _collection.Find(_ => true).ToListAsync();

            return View(methods);
        }
    }
}
