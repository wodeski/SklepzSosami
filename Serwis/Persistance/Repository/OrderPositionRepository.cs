using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Repository
{
    public class OrderPositionRepository : IOrderPositionRepository
    {

        private readonly IApplicationDbContext _serviceDbContext;
        public OrderPositionRepository(IApplicationDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }
        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsAsync()
        {
            return await _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .ToListAsync();//zle
        }

        public async Task CreateOrderPositionAsync(OrderPosition orderPosition)
        {
            await _serviceDbContext.OrderPositions.AddAsync(orderPosition);
            
        }

        public async Task DeleteOrderPositionAsync(int orderId, int userId, int productId)
        {
            var orderPositionToDelete = await _serviceDbContext.OrderPositions
                .FirstAsync(x => x.OrderId == orderId && x.ProductId == productId && x.UserId == userId);
            if (orderPositionToDelete == null)
            {
                throw new ArgumentNullException();
            }
            _serviceDbContext.OrderPositions.Remove(orderPositionToDelete);
            
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int userId)
        {
            return await _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .Where(x => x.UserId == userId && x.Order.IsCompleted == false)
                 .ToListAsync();
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId)
        {
            var positions = await _serviceDbContext.OrderPositions
                  .Include(x => x.Product)
                  .Where(x => x.UserId == userId && x.OrderId == orderId && x.Order.IsCompleted == false)
                  .ToListAsync();

            return positions;
        }

        public Task CreateOrderPositionAsync(int productId, int userId, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountOrderPositions(int orderId, string userId)
        {
            return await _serviceDbContext.OrderPositions.Where(x => x.UserId == Convert.ToInt32(userId) && x.OrderId == orderId).CountAsync();
        }
    }
}
