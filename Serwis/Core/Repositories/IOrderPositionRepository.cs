using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IOrderPositionRepository
    {
        Task DeleteOrderPositionAsync(int orderId, int userId, int productId);
        Task<IEnumerable<OrderPosition>> GetPositionsAsync();
        Task<IEnumerable<OrderPosition>> GetPositionsForUserAsync(int userId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId);
        Task CreateOrderPositionAsync(OrderPosition orderPosition);
        Task AddPositionAsync(int UserId, int ProductId);


    }
}
