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

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId)
        {
            var orders = await _serviceDbContext.Orders.Where(x => x.UserId == userId).ToListAsync();
            return orders;
        }
    }
}
