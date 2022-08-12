using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId);
        Task<string> UpdateOrderAsync(int OrderId);
        Task CreateOrderAsync(Order order);
        Task<Order> FindOrderByIdAsync(int orderId);
    }
}
