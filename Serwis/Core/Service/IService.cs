using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Core.Service
{
    public interface IService
    {
        IEnumerable<Product> Get(int categoryId = 0, string? name = null);
        string GenerateInvoice(OrderViewModel order);
        Task UpdateOrderPositionAsync(OrderPosition orderPosition, Order order);
        //string GenerateInvoice(Order order);
        //Task<string> ReportSentAsync(IEnumerable<OrderPosition> orderPositions);
        void SendMail(string emailReciever, OrderViewModel order);
        Task CreateUser(ApplicationUser user);
        Task<ApplicationUser> FindUserWithLoginCredentials(string userName, string password);
        Task<ApplicationUser> GetUser(string userName);
        Task<bool> IsUserNameFromRegisterValid(string userName);
        Task<List<OrderPosition>> SetOrderPositions(int orderId, int[] quantityOfPositions);
        Task<OrderViewModel> SetOrderForPosition(int orderId, int[] quantityOfPositions);
        Task<decimal> CheckSumValueOfProducts(List<int> productId, List<int> quantity);
        Task DeleteOrder(Guid userId, int orderId);
        Task CreateOrderPositionAsync(int productId, Guid userId, int orderId);
        Task<Order> FindOrderByUserIdAsync(Guid userId);
        Task AddOrderPositionAsync(Guid userId, int ProductId);
        Task CreateListOfCategories();
        Task CreateOrderAsync(Order order);
        Task CreateOrderPositionAsync(OrderPosition orderPosition);
        Task CreateProductAsync(Product product);
        Task DeleteOrderPositionAsync(int orderId, Guid userId, int productId);
        Task DeleteProductAsync(int id);
        Task<Order> FindOrderByIdAsync(int orderId);
        Task<Product> FindProductByIdAsync(int id);
        Task<ApplicationUser> FindUserAsync(string userName);
        Task<ApplicationUser> FindUserByIdAsync(Guid userId);
        Task<List<ProductCategory>> GetListOfProductCategories();
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(int orderId, Guid userId);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId);
        Task<IEnumerable<OrderPosition>> GetOrderPositionsAsync();
        Task<IEnumerable<OrderPosition>> GetOrderPositionsForUserAsync(Guid userId);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<bool> IsProductCategoryListEmpty();
        Task<string> UpdateOrderAsync(int OrderId);
        Task UpdateProductAsync(Product product);
        Task<bool> UserHasAnIncompleteOrder(Guid userId);
        Task<int> CountOrderPositions(int orderId, Guid userId);
    }
}