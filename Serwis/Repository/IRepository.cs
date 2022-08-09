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
        ApplicationUser FindUserById(int userId);
        Product FindProductById(int productId);
        Order FindOrderById(int orderId);

        IEnumerable<OrderPosition> GetOrderPositionsForUser(int orderId, int userId);
        IEnumerable<Order> GetOrdersForUser(int userId); 
       // Order GetOrderForUser(int userId); 
        string UpdateOrder(int OrderId);

        void CreateOrder(Order order);

    }
}