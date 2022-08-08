using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public interface IRepository
    {
        Task CreateProductAsync(Product product);
        Task DeleteItemAsync(int id);
        Task<Product> GetItemAsync(int id);
        Task<IEnumerable<Product>> GetItemsAsync();
        Product GetProduct(int id);
        Task UpdateProductAsync(Product product);
        void AddPosition(int UserId, int ProductId);
        IEnumerable<OrderPosition> GetPositions();
        IEnumerable<OrderPosition> GetPositionsForUser(int userId);
        ApplicationUser FindUser(string userName);

        IEnumerable<OrderPosition> GetOrderPositionsForUser(int orderId, int userId);
        string UpdateOrder(int OrderId);

    }
}