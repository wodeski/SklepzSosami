using Microsoft.EntityFrameworkCore;
using Serwis.Models.Domains;
using Serwis.Models.Service;

namespace Serwis.Repository
{
    public class EFRepository : IRepository
    {
        private readonly ServiceDbContext _serviceDbContext;
        private List<ProductCategory> _categories;
        public EFRepository(ServiceDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
            var categories = new List<ProductCategory>()
            {
                new ProductCategory()
             {
                 Name="Łagodny"
             },
             new ProductCategory()
             {
                 Name = "Lekko pikantny"
             },
             new ProductCategory()
             {
                 Name = "Pikantny"
             },
             new ProductCategory()
             {
                 Name = "Bardzo pikantny"
             },
             new ProductCategory()
             {
                 Name = "Piekielnie pikantny"
             }
            };

            _categories = categories;
        }

        public async Task CreateListOfCategories()
        {
            foreach (var category in _categories)
            {
                await _serviceDbContext.ProductCategories.AddAsync(category);
            }
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task<List<ProductCategory>> GetListOfProductCategories()
        {
            var categories = await _serviceDbContext.ProductCategories.ToListAsync();

            return categories;
        }

        public async Task<bool> IsProductCategoryListEmpty()
        {
            var productCategoryList = await _serviceDbContext.ProductCategories.CountAsync();
            if (productCategoryList == 0)
            {
                return true;
            }
            return false;
        }



        ///TO JEST DO PRZEROBIENIA!!!
        public async Task AddPositionAsync(int UserId, int ProductId)
        {
            //trzeba zliczyc ile zamowien jest juz jest w bd dla tego uzytkownika a nastepnie te wartosc przypisac 
            var orderId = _serviceDbContext.Orders.Count(x => x.UserId == UserId);

            //zakladamy ze moze byc tylko jedno zamowienie niezrealizowane
            var isThereAnyOrderIncomplete = await _serviceDbContext.Orders.AnyAsync(x => x.UserId == UserId && x.IsCompleted == false);
            if (!isThereAnyOrderIncomplete)// jesli nie ma zadnego nieukonczonego zamowienia to stworz nowe 
            {
                var newOrder = new Order
                {
                    UserId = UserId, //dodanie nowego zamowienia dla usera
                    IsCompleted = false
                };
                //dodac nowe zamowienie
                await _serviceDbContext.Orders.AddAsync(newOrder);
                await _serviceDbContext.SaveChangesAsync();
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
            await _serviceDbContext.OrderPositions.AddAsync(position);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _serviceDbContext.Products.AddAsync(product);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var findItem = await _serviceDbContext.Orders.SingleAsync(o => o.Id == id);
            _serviceDbContext.Orders.Remove(findItem);
            await _serviceDbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _serviceDbContext.Products.ToListAsync(); //entity framework 
        }
        public async Task<IEnumerable<OrderPosition>> GetPositionsAsync()
        {
            return await _serviceDbContext.OrderPositions
                 .Include(x => x.Product)
                 .Include(x => x.User)
                 .Include(x => x.Order)
                 .ToListAsync();//zle
        }

        public async Task UpdateProductAsync(Product product)
        {
            var findItem = _serviceDbContext.Products.Single(o => o.Id == product.Id);
            findItem.Name = product.Name;
            findItem.ImageFileName = product.ImageFileName;
            await _serviceDbContext.SaveChangesAsync();
        }
        public async Task<ApplicationUser> FindUserAsync(string userName)
        {
            //poprawic warunki
            var findUser = await _serviceDbContext.Credentials.Where(o => o.UserName == userName).FirstOrDefaultAsync();
            if (findUser == null)
            {
                return null;
            }
            return findUser;
        }
        public async Task<Product> GetProductAsync(int id)
        {
            var find = await _serviceDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (find == null)
                return null;
            return find;
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

        public async Task<string> UpdateOrderAsync(int OrderId) //zmienic nazwe
        {
            var findOrder = await _serviceDbContext.Orders.SingleAsync(x => x.IsCompleted == false && x.Id == OrderId);

            findOrder.IsCompleted = true;
            findOrder.Title = $"Order/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}";
            await _serviceDbContext.SaveChangesAsync();
            return findOrder.Title;

        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId)
        {
            var orders = await _serviceDbContext.Orders.Where(x => x.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task<ApplicationUser> FindUserByIdAsync(int userId)
        {
            var findUser = await _serviceDbContext.Credentials.Where(x => x.Id == userId).FirstAsync();
            if (findUser == null)
            {
                return null;
            }
            return findUser;
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
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task<Product> FindProductByIdAsync(int id)
        {
            var findProduct = await _serviceDbContext.Products.SingleAsync(o => o.Id == id);
            if (findProduct == null)
            {
                return null;
            }
            return findProduct;
        }
        public async Task CreateOrderPositionAsync(OrderPosition orderPosition)
        {
            await _serviceDbContext.OrderPositions.AddAsync(orderPosition);
            await _serviceDbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderPositionAsync(int orderId, int userId, int productId)
        {
            var orderPositionToDelete =await _serviceDbContext.OrderPositions
                .FirstAsync(x => x.OrderId == orderId && x.ProductId == productId && x.UserId == userId);
            if(orderPositionToDelete == null)
            {
                throw new ArgumentNullException();
            }
             _serviceDbContext.OrderPositions.Remove(orderPositionToDelete);
             await _serviceDbContext.SaveChangesAsync();
        }
    }
}
