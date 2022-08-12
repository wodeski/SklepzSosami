using Serwis.Core;
using Serwis.Core.Service;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Service
{
    public class Service : IService
    {
        private readonly IUnitOfWork _unitOfWork;

        public Service(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task CreateListOfCategories()
        {
            await _unitOfWork.ProductCategory.CreateListOfCategories();
        }

        public async Task<List<ProductCategory>> GetListOfProductCategories()
        {
            return await _unitOfWork.ProductCategory.GetListOfProductCategories();
        }

        public async Task<bool> IsProductCategoryListEmpty()
        {
            return await _unitOfWork.ProductCategory.IsProductCategoryListEmpty();
        }

        public async Task AddPositionAsync(int UserId, int ProductId)
        {
            await _unitOfWork.OrderPosition.AddPositionAsync(UserId, ProductId);
        }


        public async Task<IEnumerable<OrderPosition>> GetPositionsAsync()
        {
            return await _unitOfWork.OrderPosition.GetPositionsAsync();
        }

        public async Task CreateOrderPositionAsync(OrderPosition orderPosition)
        {
            await _unitOfWork.OrderPosition.CreateOrderPositionAsync(orderPosition);
        }

        public async Task DeleteOrderPositionAsync(int orderId, int userId, int productId)
        {
            await _unitOfWork.OrderPosition.DeleteOrderPositionAsync(orderId, userId, productId);
        }

        public async Task<IEnumerable<OrderPosition>> GetPositionsForUserAsync(int userId)
        {
            return await _unitOfWork.OrderPosition.GetPositionsForUserAsync(userId);
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
        }
        public async Task<string> UpdateOrderAsync(int OrderId) //zmienic nazwe
        {
            return await _unitOfWork.Order.UpdateOrderAsync(OrderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(int userId)
        {
            return await _unitOfWork.Order.GetOrdersForUserAsync(userId);
        }
        public async Task CreateProductAsync(Product product)
        {
            await _unitOfWork.Product.CreateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _unitOfWork.Product.DeleteProductAsync(id);
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _unitOfWork.Product.GetProductsAsync();
        }
        public async Task UpdateProductAsync(Product product)
        {
            await _unitOfWork.Product.UpdateProductAsync(product);
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

    }
}
