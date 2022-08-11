using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public interface IRepository
    {
        Task DeleteOrderPositionAsync(int orderId, int userId, int productId);
        Task CreateListOfCategories();
        Task<List<ProductCategory>> GetListOfProductCategories();
        Task<bool> IsProductCategoryListEmpty();
        Task CreateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task UpdateProductAsync(Product product);
        Task AddPositionAsync(int UserId, int ProductId);
        Task<IEnumerable<OrderPosition>> GetPositionsAsync();
        Task<IEnumerable<OrderPosition>> GetPositionsForUserAsync(int userId);
        Task<ApplicationUser> FindUserAsync(string userName);
        Task<ApplicationUser> FindUserByIdAsync(int userId);
        Task<Product> FindProductByIdAsync(int productId);
        Task<Order> FindOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId); 
        Task<string> UpdateOrderAsync(int OrderId);
        Task CreateOrderAsync(Order order);
        Task CreateOrderPositionAsync(OrderPosition orderPosition);

    }
}