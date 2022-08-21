using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId);
        Task<string> UpdateOrderAsync(int OrderId);
        Task CreateOrderAsync(Order order);
        Task<Order> FindOrderByIdAsync(int orderId);
        Task<Order> FindOrderByUserIdAsync(Guid userId);
        Task DeleteOrder(Guid userId, int orderId);
    }
}
