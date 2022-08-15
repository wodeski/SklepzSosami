using Serwis.Core;
using Serwis.Core.Service;
using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Persistance.Service
{
    public class Service : IService
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<ProductCategory> _categories;
        public Service(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }


        public async Task CreateListOfCategories()
        {
            await _unitOfWork.ProductCategory.CreateListOfCategories();
            _unitOfWork.Complete();
        }

        public async Task<List<ProductCategory>> GetListOfProductCategories()
        {
            return await _unitOfWork.ProductCategory.GetListOfProductCategories();
        }

        public async Task<bool> IsProductCategoryListEmpty()
        {
            return await _unitOfWork.ProductCategory.IsProductCategoryListEmpty();
        }

        public async Task AddOrderPositionAsync(int userId, int productId)
        {
            var userIncompleteOrder = await _unitOfWork.Order.FindOrderByUserIdAsync(userId);
            //sprawdza czy jest jakies zamowienie nieukonczone
            if (userIncompleteOrder == null)// jesli nie ma zadnego nieukonczonego zamowienia to stworz nowe 
            {
                await CreateNewOrder(userId);
                userIncompleteOrder = await _unitOfWork.Order.FindOrderByUserIdAsync(userId);
            }
            
            var orderPosition = PrepareOrderPosition(userId, productId, userIncompleteOrder);
            
            try
            {
                await _unitOfWork.OrderPosition.CreateOrderPositionAsync(orderPosition);
                _unitOfWork.Complete();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private static OrderPosition PrepareOrderPosition(int userId, int productId, Order userIncompleteOrder)
        {
            return new OrderPosition
            {
                OrderId = userIncompleteOrder.Id,
                UserId = userId,
                ProductId = productId,
            };
        }

        private async Task CreateNewOrder(int userId)
        {
            var newOrder = new Order
            {
                UserId = userId, //dodanie nowego zamowienia dla usera
                IsCompleted = false
            };
            //dodac nowe zamowienie
            await _unitOfWork.Order.CreateOrderAsync(newOrder);
            _unitOfWork.Complete();
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsAsync()
        {
            return await _unitOfWork.OrderPosition.GetOrderPositionsAsync();
        }

        public async Task CreateOrderPositionAsync(OrderPosition orderPosition)
        {
            await _unitOfWork.OrderPosition.CreateOrderPositionAsync(orderPosition);
            _unitOfWork.Complete();

        }

        public async Task DeleteOrderPositionAsync(int orderId, int userId, int productId)
        {
            await _unitOfWork.OrderPosition.DeleteOrderPositionAsync(orderId, userId, productId);
            _unitOfWork.Complete();

        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int userId)
        {
            return await _unitOfWork.OrderPosition.GetOrderPositionsForUserAsync(userId);
        }

        public async Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, int userId)
        {
            return await _unitOfWork.OrderPosition.GetOrderPositionsForUserAsync(orderId, userId);
        }

        public async Task<Order> FindOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.Order.FindOrderByIdAsync(orderId);
        }
        public async Task CreateOrderAsync(Order order)
        {
            await _unitOfWork.Order.CreateOrderAsync(order);
            _unitOfWork.Complete();

        }
        public async Task<string> UpdateOrderAsync(int OrderId) //zmienic nazwe
        {
            var title = await _unitOfWork.Order.UpdateOrderAsync(OrderId);
            _unitOfWork.Complete();
            return title;
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId)
        {
            return await _unitOfWork.Order.GetOrdersForUserAsync(userId);
        }
        public async Task CreateProductAsync(Product product)
        {
            await _unitOfWork.Product.CreateProductAsync(product);
            _unitOfWork.Complete();

        }

        public async Task DeleteProductAsync(int id)
        {
            await _unitOfWork.Product.DeleteProductAsync(id);
            _unitOfWork.Complete();

        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _unitOfWork.Product.GetProductsAsync();
        }
        public async Task UpdateProductAsync(Product product)
        {
            await _unitOfWork.Product.UpdateProductAsync(product);
            _unitOfWork.Complete();

        }
        public async Task<Product> GetProductAsync(int id)
        {
            return await _unitOfWork.Product.GetProductAsync(id);
        }

        public async Task<Product> FindProductByIdAsync(int id)
        {
            return await _unitOfWork.Product.FindProductByIdAsync(id);
        }

        public async Task<ApplicationUser> FindUserAsync(string userName)
        {
            return await _unitOfWork.User.FindUserAsync(userName);
        }
        public async Task<ApplicationUser> FindUserByIdAsync(int userId)
        {
            return await _unitOfWork.User.FindUserByIdAsync(userId);
        }

        public async Task<bool> UserHasAnIncompleteOrder(int userId)
        {
            return await _unitOfWork.User.UserHasAnIncompleteOrder(userId);
        }

        public async Task<Order> FindOrderByUserIdAsync(int userId)
        {
            return await _unitOfWork.Order.FindOrderByIdAsync(userId);
        }

        public async Task CreateOrderPositionAsync(int productId, int userId, int orderId)
        {
                var orderPosition = new OrderPosition
                {
                    OrderId = orderId,
                    UserId = userId,
                    ProductId = productId

                };
            await _unitOfWork.OrderPosition.CreateOrderPositionAsync(orderPosition);
            _unitOfWork.Complete();
        }

        public async Task<int> CountOrderPositions(int orderId, string userId)
        {
            return await _unitOfWork.OrderPosition.CountOrderPositions(orderId, userId);
        }

        public async Task DeleteOrder(int userId, int orderId)
        {
            await _unitOfWork.Order.DeleteOrder(userId, orderId);
            _unitOfWork.Complete();
        }

        public Task<decimal> CheckSumValueOfProducts(List<int> productId, List<int> quantity)
        {
            return _unitOfWork.Product.CheckSumValueOfProducts(productId, quantity);
        }


        public async Task<OrderViewModel> SetOrderForPosition(int userId, int orderId, int[] quantityOfPositions)
        {
            var orderPositions = SetOrderPositions(orderId, quantityOfPositions);

            List<int> productId, quantity;
            int[] qop;
            SplitToLists(quantityOfPositions, out productId, out quantity, out qop);

            var sumvalue = await CheckSumValueOfProducts(productId, quantity);

            var random = new Random();
            var rnd = random.Next(0, 1000);
            var ordervm = new OrderViewModel
            {
                FullPrice = sumvalue,
                OrderPositions = orderPositions,
                Id = orderId,
                UserId = userId,
                IsCompleted = false,
                Title = $"fak/{userId}/{DateTime.Now.ToString("dd-mm-yyyy-mm-ss")}{rnd}"

            };

            return ordervm;
            //problem polega na tym ze nie jestemy w stanie przekazac ani sumy ani listy z pozycjami do zamownienia

        }

        public List<OrderPosition> SetOrderPositions(int orderId, int[] quantityOfPositions)
        {
            List<int> productId, quantity;
            int[] qop;
            SplitToLists(quantityOfPositions, out productId, out quantity, out qop);

            //Dla kazdego produktu trzeba znalesc cene z bazy danych nastepnie te cenen pomoznozyc przez ilosc 
            List<OrderPosition> orderPositions = UpdateOrderPositionsAndAddToList(orderId, productId, quantity, qop);

            return orderPositions;
        }

        private static List<OrderPosition> UpdateOrderPositionsAndAddToList(int orderId, List<int> productId, List<int> quantity, int[] qop)
        {
            List<OrderPosition> orderPositions = new List<OrderPosition>();

            for (int i = 0; i < (qop.Length / 2); i++)
            {
                orderPositions.Add(new OrderPosition
                {
                    ProductId = productId[i],
                    Quantity = quantity[i],
                    OrderId = orderId


                });

            }

            return orderPositions;
        }
        private static void SplitToLists(int[] quantityOfPositions, out List<int> productId, out List<int> quantity, out int[] qop)
        {
            productId = new List<int>();
            quantity = new List<int>();
            qop = new int[quantityOfPositions.Length];
            qop = quantityOfPositions;
            for (int i = 0; i < qop.Length; i++)
            {
                if (i % 2 == 0)
                    productId.Add(qop[i]);
                else
                    quantity.Add(qop[i]);
            }
        }

    }
}
