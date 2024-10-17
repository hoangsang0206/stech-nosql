using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMongoCollection<Menu> _collection;

        public MenuViewComponent(StechDbContext context) => _collection = context.GetCollection<Menu>("Menu");

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Menu> menu = await _collection.Find(_ => true).ToListAsync();

            return View(menu);
        }
    }
}
