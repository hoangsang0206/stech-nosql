using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class CategoryUtils
    {
        public static IEnumerable<Category> Sort(this IEnumerable<Category> categories, string? sort_by)
        {
            if (sort_by == null) { return categories; }

            return sort_by switch
            {
                "name" => categories.OrderBy(c => c.CategoryName),
                "name_desc" => categories.OrderByDescending(c => c.CategoryName),
                _ => categories,
            };
        }

        public static IEnumerable<Category> Paginate(this IEnumerable<Category> categories, int page, int itemsPerPage)
        {
            return categories.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
        }
    }
}
