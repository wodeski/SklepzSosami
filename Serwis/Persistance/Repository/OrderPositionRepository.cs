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

        public async Task DeleteOrderPositionAsync(int orderId, Guid userId, int productId)
        {
            var orderPositionToDelete = await _serviceDbContext.OrderPositions
                .FirstAsync(x => x.OrderId == orderId && x.ProductId == productId && x.UserId == userId);
            if (orderPositionToDelete == null)
            {
                throw new ArgumentNullException();
            }
            _serviceDbContext.OrderPositions.Remove(orderPositionToDelete);
            
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(Guid userId)
        {
            return await _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .Where(x => x.UserId == userId && x.Order.IsCompleted == false)
                 .ToListAsync();
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, Guid userId)
        {
            var positions = await _serviceDbContext.OrderPositions
                  .Include(x => x.Product)
                  .Where(x => x.UserId == userId && x.OrderId == orderId && x.Order.IsCompleted == false)
                  .ToListAsync();

            return positions;
        }
        //dla anonimowego uzytkownika
        public async Task UpdateOrderPositionAsync(OrderPosition orderPosition, Order order)//utworzyc metode ktora zaktualizuje wszystkie elementy z lity
        {
            OrderPosition findOrderPositionToUpdate;

            try
            {
                var findOrderId = await _serviceDbContext.Orders.SingleAsync(x => x.Title == order.Title && x.UserId == order.UserId);

                findOrderPositionToUpdate = new OrderPosition
                {
                    OrderId = findOrderId.Id,
                    ProductId = orderPosition.ProductId,
                    Quantity = orderPosition.Quantity,
                    UserId = order.UserId
                };


                await _serviceDbContext.OrderPositions.AddAsync(findOrderPositionToUpdate);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            

        }

        public async Task<int> CountOrderPositions(int orderId, Guid userId)
        {
            return await _serviceDbContext.OrderPositions
                .Where(x => x.UserId == userId && x.OrderId == orderId)
                .CountAsync();
        }

    }
}
