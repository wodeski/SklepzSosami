using Serwis.Models.Domains;

namespace Serwis.Core.Service
{
    public interface IService
    {
        Task AddPositionAsync(int UserId, int ProductId);
        Task CreateListOfCategories();
        Task CreateOrderAsync(Order order);
        Task CreateOrderPositionAsync(OrderPosition orderPosition);
        Task CreateProductAsync(Product product);
        Task DeleteOrderPositionAsync(int orderId, int userId, int productId);
        Task DeleteProductAsync(int id);
        Task<Order> FindOrderByIdAsync(int orderId);
        Task<Product> FindProductByIdAsync(int id);
        Task<ApplicationUser> FindUserAsync(string userName);
        Task<ApplicationUser> FindUserByIdAsync(int userId);
        Task<List<ProductCategory>> GetListOfProductCategories();
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId);
        Task<IEnumerable<OrderPosition>> GetPositionsAsync();
        Task<IEnumerable<OrderPosition>> GetPositionsForUserAsync(int userId);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<bool> IsProductCategoryListEmpty();
        Task<string> UpdateOrderAsync(int OrderId);
        Task UpdateProductAsync(Product product);
        Task<bool> UserHasAnIncompleteOrder(int userId);
    }
}