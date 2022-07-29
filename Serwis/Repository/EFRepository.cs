using Microsoft.EntityFrameworkCore;
using Serwis.Models;
using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public class EFRepository : IRepository
    {
        private readonly ServiceDbContext _serviceDbContext;
        public EFRepository(ServiceDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }
        public async Task CreateItemAsync(Order order)
        {
            await _serviceDbContext.AddAsync(order);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var findItem = _serviceDbContext.Orders.Single(o => o.Id == id);
            _serviceDbContext.Orders.Remove(findItem);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task<Order> GetItemAsync(int id)
        {
            return await _serviceDbContext.Orders.SingleAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetItemsAsync()
        {
            return await _serviceDbContext.Orders.ToListAsync(); //entity framework 
        }

        public async Task UpdateItemAsync(Order order)
        {
            var findItem = _serviceDbContext.Orders.Single(o => o.Id == order.Id);
            findItem.Name = order.Name;
            await _serviceDbContext.SaveChangesAsync();
        }
    }
}
