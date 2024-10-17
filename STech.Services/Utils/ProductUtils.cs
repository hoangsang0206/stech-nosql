using STech.Data.Models;

namespace STech.Services.Utils
{
    public static class ProductUtils
    {
        public static IEnumerable<Product> SelectProduct(this IEnumerable<Product> products)
        {
            return products.Select(p => new Product()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.ImgId).Take(1).ToList(),
                WarehouseProducts = p.WarehouseProducts,
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                DateDeleted = p.DateDeleted,
            });
        }

        public static IEnumerable<Product> SelectProduct(this IEnumerable<Product> products, string? warehouseId)
        {
            return products.Select(p => new Product()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                OriginalPrice = p.OriginalPrice,
                Price = p.Price,
                ProductImages = p.ProductImages.OrderBy(pp => pp.ImgId).Take(1).ToList(),
                WarehouseProducts = warehouseId == null ? p.WarehouseProducts
                        : p.WarehouseProducts.Where(wp => wp.WarehouseId == warehouseId).ToList(),
                BrandId = p.BrandId,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                DateDeleted = p.DateDeleted,
            }).ToList();
        }

        public static IEnumerable<Product> Sort(this IEnumerable<Product> products, string? value)
        {
            if(value == null)
            {
                return products;
            }

            IEnumerable<Product> sortedProduct = new List<Product>();

            if (value == "price-ascending")
            {
                sortedProduct = products.OrderBy(t => t.Price).ToList();
            }
            else if (value == "price-descending")
            {
                sortedProduct = products.OrderByDescending(t => t.Price).ToList();
            }
            else if (value == "name-az")
            {
                sortedProduct = products.OrderBy(t => t.ProductName).ToList();
            }
            else if (value == "name-za")
            {
                sortedProduct = products.OrderByDescending(t => t.ProductName).ToList();
            }
            else
            {
                sortedProduct = products.OrderBy(sp => Guid.NewGuid()).ToList();
            }

            return sortedProduct;
        }

        public static IEnumerable<Product> Pagnigate(this IEnumerable<Product> products, int page, int numToTake)
        {
            if(page <= 0)
            {
                page = 1;
            }

            int noOfProductToSkip = (page - 1) * numToTake;

            products = products.Skip(noOfProductToSkip).Take(numToTake).ToList();

            return products;
        }

        public static IEnumerable<Product> Filter(this IEnumerable<Product> products, string? brands, string? categories, string? status, string? price_range)
        {
            string[] filter_brands = brands?.Split(',') ?? [];
            string[] filter_categories = categories?.Split(',') ?? [];
            string[] filter_price_range = price_range?.Split(',') ?? [];

            if (filter_brands.Length > 0)
            {
                products = products.Where(p => filter_brands.Contains(p.BrandId)).ToList();
            }

            if (filter_categories.Length > 0)
            {
                products = products.Where(p => filter_categories.Contains(p.CategoryId)).ToList();
            }

            if(filter_price_range.Length >= 2)
            {
                products = products.Where(p => p.Price >= Convert.ToDecimal(filter_price_range[0]) && p.Price <= Convert.ToDecimal(filter_price_range[1])).ToList();
            }

            switch (status)
            {
                case "in-stock":
                    products = products.Where(p => p.IsActive == true && p.WarehouseProducts?.Sum(wp => wp.Quantity) > 0).ToList();
                    break;

                case "out-of-stock-soon":
                    products = products
                        .Where(p => p.IsActive == true)
                        .Where(p =>
                        {
                            int totalQuantity = p.WarehouseProducts?.Sum(wp => wp.Quantity) ?? 0;

                            return totalQuantity > 0 && totalQuantity <= 5;
                        
                        })
                        .ToList();
                    break;

                case "out-of-stock":
                    products = products.Where(p => p.IsActive == true && p.WarehouseProducts?.Sum(wp => wp.Quantity) <= 0).ToList();
                    break;

                case "inactive":
                    products = products.Where(p => p.IsActive == false && p.IsDeleted == false).ToList();
                    break;

                case "activated":
                    products = products.Where(p => p.IsActive == true).ToList();
                    break;

                case "deleted":
                    products = products.Where(p => p.IsDeleted == true).ToList();
                    break;
                default:
                    products = products.Where(p => p.IsActive == true).ToList();
                    break;
            }

            return products;
        }
    }
}
