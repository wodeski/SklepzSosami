using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _serviceDbContext;
        public ProductRepository(IApplicationDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }

        public async Task CreateProductAsync(Product product)
        {
            await _serviceDbContext.Products.AddAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var findItem = await _serviceDbContext.Products.SingleAsync(o => o.Id == id);
            _serviceDbContext.Products.Remove(findItem);
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _serviceDbContext.Products.ToListAsync(); //entity framework 
        }
        public async Task UpdateProductAsync(Product product)
        {
            var findItem = _serviceDbContext.Products.Single(o => o.Id == product.Id);
            findItem.Name = product.Name;
            findItem.ImageFileName = product.ImageFileName;
        }
        public async Task<Product> GetProductAsync(int id)
        {
            var find = await _serviceDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (find == null)
                return null;
            return find;
        }

        public async Task<Product> FindProductByIdAsync(int id)
        {
            var findProduct = await _serviceDbContext.Products.SingleAsync(o => o.Id == id);
            if (findProduct == null)
            {
                return null;
            }
            return findProduct;
        }

        public async Task<decimal> CheckSumValueOfProducts(List<int> productId, List<int> quantity)
        {
            decimal sum = 0;
            for (int i = 0; i < productId.Count; i++)
            {
                var produkt = await _serviceDbContext.Products.SingleAsync(x => x.Id == productId[i]);
                sum = sum + (produkt.Price * quantity[i]);
            }
            return sum;

        }
    }
}
