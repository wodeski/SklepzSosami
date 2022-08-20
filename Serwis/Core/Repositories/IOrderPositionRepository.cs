using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IOrderPositionRepository
    {
        Task UpdateOrderPositionAsync(OrderPosition orderPosition, Order order);
        Task DeleteOrderPositionAsync(int orderId, int userId, int productId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsAsync();
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int userId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId);
        Task CreateOrderPositionAsync(int productId, int userId, int orderId);
        Task<int> CountOrderPositions(int orderId, string userId);
    }
}
