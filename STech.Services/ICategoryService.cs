using STech.Data.Models;
using STech.Data.MongoViewModels;

namespace STech.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll(bool isExcept);
        Task<IEnumerable<CategoryMVM>> GetRandomWithProducts(int numCategories, int numProducts);
        Task<(IEnumerable<CategoryMVM>, int)> GetAllWithProducts(string? sort_by, int page = 1);
        Task<Category?> GetOne(string id);

        Task<bool> Create(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(string id);
    }
}
