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
            var pozycja = new OrderPosition
            {
                OrderId = 1,
                UserId = UserId,
                ProductId = ProductId,
            };
            //warunek gdy nie ma zamówień
            //warunek gdy nie ma uzytkownika
            _serviceDbContext.OrderPositions.Add(pozycja);
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
                return new ApplicationUser();
            }
            return findUser;
        }

        public Product GetProduct(int id)
        {
            var find = _serviceDbContext.Products.FirstOrDefault(x => x.Id == id);
            if (find == null)
                throw new ArgumentNullException();
            return find;
        }

        public IEnumerable<OrderPosition> GetPositionsForUser(int userId)
        {
            return _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .Where(x=>x.UserId == userId)
                 .ToList();//zle
        }
    }
}
