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

        public void AddPosition(int UserId, int ProductId)
        {
            //trzeba zliczyc ile zamowien jest juz jest w bd dla tego uzytkownika a nastepnie te wartosc przypisac 
            var orderId = _serviceDbContext.Orders.Count(x => x.UserId == UserId);

            //zakladamy ze moze byc tylko jedno zamowienie niezrealizowane
            var isThereAnyOrderIncomplete = _serviceDbContext.Orders.Any(x=>x.UserId == UserId && x.IsCompleted == false);
            if(!isThereAnyOrderIncomplete)// jesli nie ma zadnego nieukonczonego zamowienia to stworz nowe 
            {
                var newOrder = new Order
                {
                    UserId = UserId, //dodanie nowego zamowienia dla usera
                    IsCompleted = false
                };
                //dodac nowe zamowienie
                _serviceDbContext.Orders.Add(newOrder);
                _serviceDbContext.SaveChanges();
                orderId++;
            }
            //zamówienie musi sie tworrzyc tylko raz podczas gdy pozycje do zamowienia mozna dolaczac ile chcemy dopóki zamowienie nie bedzie mialo statusu zrealizowane 

            //sprawdzic czy sa jakies niezrealizowane zamowienia jesli tak dodawac do tych zamowien
            var position = new OrderPosition
            {
                OrderId = orderId,
                UserId = UserId,
                ProductId = ProductId,
            };
            //warunek gdy nie ma zamówień
            //warunek gdy nie ma uzytkownika
            _serviceDbContext.OrderPositions.Add(position);
            _serviceDbContext.SaveChanges();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _serviceDbContext.Products.AddAsync(product);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var findItem = _serviceDbContext.Orders.Single(o => o.Id == id);
            _serviceDbContext.Orders.Remove(findItem);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task<Product> GetItemAsync(int id)
        {
            return await _serviceDbContext.Products.SingleAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Product>> GetItemsAsync()
        {
            return await _serviceDbContext.Products.ToListAsync(); //entity framework 
        }

        public IEnumerable<OrderPosition> GetPositions()
        {
           return _serviceDbContext.OrderPositions
                .Include(x=>x.Product)
                .Include(x=>x.User)
                .Include(x=>x.Order)
                .ToList();//zle
            
        }

        public async Task UpdateProductAsync(Product product)
        {
            var findItem = _serviceDbContext.Products.Single(o => o.Id == product.Id);
            findItem.Name = product.Name;
            findItem.ImageFileName = product.ImageFileName;
            await _serviceDbContext.SaveChangesAsync();
        }
        public ApplicationUser FindUser(string userName)
        {
            //poprawic warunki
            var findUser = _serviceDbContext.Credentials.Where(o => o.UserName == userName).FirstOrDefault();
            if(findUser == null)
            {
                return null;
            }
            return findUser;
        }

        public Product GetProduct(int id)
        {
            var find = _serviceDbContext.Products.FirstOrDefault(x => x.Id == id);
            if (find == null)
                return null;
            return find;
        }

        public IEnumerable<OrderPosition> GetPositionsForUser(int userId)
        {
            return _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .Where(x=>x.UserId == userId && x.Order.IsCompleted == false)
                 .ToList();//zle
        }

        public IEnumerable<OrderPosition> GetOrderPositionsForUser(int orderId, int userId)
        {
            var positions = _serviceDbContext.OrderPositions
                  .Include(x => x.Product)
                  .Where(x => x.UserId == userId && x.OrderId==orderId && x.Order.IsCompleted == false)
                  .ToList();//zle

            var positionsTest = _serviceDbContext.Orders
                  .Include(x => x.OrderPositions)
                  .Single(x => x.UserId == userId && x.Id == orderId && x.IsCompleted == false);
                  

            return positions;
        }

        public string UpdateOrder(int OrderId)
        {
            var findOrder = _serviceDbContext.Orders.Single(x => x.IsCompleted == false && x.Id == OrderId);

            findOrder.IsCompleted = true;
            findOrder.Title = $"Order/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}";    
            _serviceDbContext.SaveChanges();
            return findOrder.Title;


        }
    }
}
