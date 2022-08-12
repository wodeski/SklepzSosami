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
        public async Task AddPositionAsync(int UserId, int ProductId)
        {
            try
            {

                //zamówienie musi sie tworrzyc tylko raz podczas gdy pozycje do zamowienia mozna dolaczac ile chcemy dopóki zamowienie nie bedzie mialo statusu zrealizowane 

                //sprawdzic czy sa jakies niezrealizowane zamowienia jesli tak dodawac do tych zamowien
                var position = new OrderPosition
                {
                    OrderId = 1,
                    UserId = UserId,
                    ProductId = ProductId,
                };
                //warunek gdy nie ma zamówień
                //warunek gdy nie ma uzytkownika
                await _serviceDbContext.OrderPositions.AddAsync(position);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }


        public async Task<IEnumerable<OrderPosition>> GetPositionsAsync()
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

        public async Task<IEnumerable<OrderPosition>> GetPositionsForUserAsync(int userId)
        {
            return await _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .Where(x => x.UserId == userId && x.Order.IsCompleted == false)
                 .ToListAsync();//zle
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId)
        {
            var positions = await _serviceDbContext.OrderPositions
                  .Include(x => x.Product)
                  .Where(x => x.UserId == userId && x.OrderId == orderId && x.Order.IsCompleted == false)
                  .ToListAsync();

            return positions;
        }
    }
}
