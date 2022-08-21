using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IOrderPositionRepository
    {
        Task UpdateOrderPositionAsync(OrderPosition orderPosition, Order order);
        Task DeleteOrderPositionAsync(int orderId, Guid userId, int productId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsAsync();
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(Guid userId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, Guid userId);
        Task CreateOrderPositionAsync(int productId, Guid userId, int orderId);
        Task<int> CountOrderPositions(int orderId, Guid userId);
    }
}
