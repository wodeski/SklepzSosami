using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public interface IRepository
    {
        Task CreateItemAsync(Order order);
        Task DeleteItemAsync(int id);
        Task<Order> GetItemAsync(int id);
        Task<IEnumerable<Order>> GetItemsAsync();
        Task UpdateItemAsync(Order order);
    }
}