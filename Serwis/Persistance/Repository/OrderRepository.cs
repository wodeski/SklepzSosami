using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IApplicationDbContext _serviceDbContext;
        public OrderRepository(IApplicationDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }
        public async Task<Order> FindOrderByIdAsync(int orderId)
        {
            var findOrder = await _serviceDbContext.Orders.Where(x => x.Id == orderId).FirstAsync();
            if (findOrder == null)
                return null;
            return findOrder;
        }
        public async Task CreateOrderAsync(Order order)
        {
            await _serviceDbContext.Orders.AddAsync(order);
        }
        public async Task<string> UpdateOrderAsync(int OrderId) //zmienic nazwe
        {
            var findOrder = await _serviceDbContext.Orders.SingleAsync(x => x.IsCompleted == false && x.Id == OrderId);

            findOrder.IsCompleted = true;
            findOrder.Title = $"Order/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}";
            return findOrder.Title;

        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId)
        {
            return await _serviceDbContext.Orders
                .Include(x=>x.OrderPositions)
                .Where(x => x.UserId == userId && x.IsCompleted == true)
                .ToListAsync();
        }

        public async Task<Order> FindOrderByUserIdAsync(Guid userId)
        {
            var order = await _serviceDbContext.Orders.Where(x => x.UserId == userId && x.IsCompleted == false).FirstOrDefaultAsync();
            if (order == null)
                return null;
            return order;
        }

        public async Task DeleteOrder(Guid userId, int orderId)
        {
            var findOrderToDelete = await _serviceDbContext.Orders.SingleAsync(x => x.UserId == userId && x.Id == orderId);
             _serviceDbContext.Orders.Remove(findOrderToDelete);
        }
    }
}
