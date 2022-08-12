using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface ICategoryRepository
    {

        Task CreateListOfCategories();
        Task<List<ProductCategory>> GetListOfProductCategories();
        Task<bool> IsProductCategoryListEmpty();







    }
}