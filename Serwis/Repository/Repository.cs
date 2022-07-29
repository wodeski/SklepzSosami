using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public class Repository //: //IRepository
    {

        private readonly List<Order> _orderList = new()
            {
                new Order
                {
                    Id = 1,
                    Name = "Pizza"
                },
                new Order
                {
                    Id = 2,
                    Name = "Burger"
                },
                new Order
                {
                    Id = 3,
                    Name = "Zupa"
                }
        };

        public async Task<IEnumerable<Order>> GetItemsAsync()
        {

            return await Task.FromResult(_orderList);
        }

        public async Task<Order> GetItemAsync(int id)
        {
            var order = _orderList.Where(x => x.Id == id).FirstOrDefault();
            //zwraca gotowe zadanie z wartoscia items 

            if (order != null)
                return await Task.FromResult(order);
            return new Order(); //zle ale chuj
        }
        public async Task CreateItemAsync(Order order)
        {
            _orderList.Add(order);
            await Task.CompletedTask;
        }
        public async Task UpdateItemAsync(Order order)
        {
            var index = _orderList.FindIndex(x => x.Id == order.Id);
            _orderList[index] = order;
            await Task.CompletedTask;
        }
        public async Task DeleteItemAsync(int id)
        {
            var index = _orderList.FindIndex(x => x.Id == id);
            _orderList.RemoveAt(index);
            await Task.CompletedTask;
        }

    }
}
