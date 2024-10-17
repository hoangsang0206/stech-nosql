using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using STech.Data.Models;

namespace STech.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IMongoCollection<SubHeader> _subHeaderCollection;
        private readonly IMongoCollection<Category> _categoryCollection;

        public HeaderViewComponent(StechDbContext context)
        {
            _subHeaderCollection = context.GetCollection<SubHeader>("SubHeaders");
            _categoryCollection = context.GetCollection<Category>("Categories");
        }


        public async Task<IViewComponentResult> InvokeAsync(string type = "")
        {
            switch(type)
            {
                case "subheader":
                    IEnumerable<SubHeader> subHeader = await _subHeaderCollection.Find(_ => true).ToListAsync();
                    return View(subHeader);
                case "mobile-subheader":
                    IEnumerable<Category> categories = await _categoryCollection.Find(c => c.CategoryId != "khac").ToListAsync();
                    categories = categories.Select(c => new Category()
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                    }).ToList();

                    return View("MobileSubHeader", categories);
            }

            return View();
        }
    }
}
