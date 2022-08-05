using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public interface IRepository
    {
        Task CreateProductAsync(Product product);
        Task DeleteItemAsync(int id);
        Task<Product> GetItemAsync(int id);
        Task<IEnumerable<Product>> GetItemsAsync();
        Task UpdateProductAsync(Product product);
        void AddPosition(int UserId, int ProductId);
        IEnumerable<OrderPosition> GetPositions();
        ApplicationUser FindUser(string userName);

    }
}