using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class BrandUtils
    {
        public static IEnumerable<Brand> Sort(this IEnumerable<Brand> brands, string? sort_by)
        {
            if(sort_by == "name_desc")
            {
                return brands.OrderByDescending(b => b.BrandName).ToList();
            }

            return brands.OrderBy(b => b.BrandName).ToList();
        }

        public static IEnumerable<Brand> Paginate(this IEnumerable<Brand> brands, int page, int itemsPerPage)
        {
            return brands.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
        }
    }
}
