using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private List<ProductCategory> _categories;
        public CategoryRepository(IApplicationDbContext serviceDbContext)
        {
            _applicationDbContext = serviceDbContext;
            var categories = new List<ProductCategory>()
            {
                new ProductCategory()
             {
                 Name="Łagodny"
             },
             new ProductCategory()
             {
                 Name = "Lekko pikantny"
             },
             new ProductCategory()
             {
                 Name = "Pikantny"
             },
             new ProductCategory()
             {
                 Name = "Bardzo pikantny"
             },
             new ProductCategory()
             {
                 Name = "Piekielnie pikantny"
             }
            };

            _categories = categories;
        }
        public async Task CreateListOfCategories()
        {
            foreach (var category in _categories)
            {
                await _applicationDbContext.ProductCategories.AddAsync(category);
            }
        }

        public async Task<List<ProductCategory>> GetListOfProductCategories()
        {
            var categories = await _applicationDbContext.ProductCategories.ToListAsync();

            return categories;
        }

        public async Task<bool> IsProductCategoryListEmpty()
        {
            var productCategoryList = await _applicationDbContext.ProductCategories.CountAsync();
            if (productCategoryList == 0)
            {
                return true;
            }
            return false;
        }

    }
}
