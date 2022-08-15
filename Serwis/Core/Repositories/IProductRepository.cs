using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IProductRepository
    {
        Task CreateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task UpdateProductAsync(Product product);
        Task<Product> FindProductByIdAsync(int id);
        Task<decimal> CheckSumValueOfProducts(List<int> productId, List<int> quantity);
    }
}
