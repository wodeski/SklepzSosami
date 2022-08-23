using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Converter
{
    public static class ModelToViewModel 
    {
        public static ProductViewModel PrepareProductViewModel(this Product product) // obracowac klase dla view modeli
        {
            var productVM = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                ImageFileName = product.ImageFileName,
                Price = product.Price,
                OrderPositions = product.OrderPositions,
                CategoryId = product.CategoryId
            };

            return productVM;
        }

        public static ApplicationUserViewModel PrepareApplicationUserViewModel(this ApplicationUser user) // obracowac klase dla view modeli
        {
            var userVM = new ApplicationUserViewModel
            {
                Id = user.Id,
                Password = user.Password,
                UserName = user.UserName,
                CreatedDate = user.CreatedDate,
                Email = user.Email,
                Orders = user.Orders


            };

            return userVM;
        }
        public static OrderViewModel PrepareOrderViewModel(this Order order) // obracowac klase dla view modeli
        {
            var orderVM = new OrderViewModel
            {
                Id = order.Id,
                Title = order.Title,
                UserId = order.UserId,
                OrderPositions = order.OrderPositions
            };

            return orderVM;
        }
        public static OrderPositionViewModel PrepareOrderPositionViewModel(this OrderPosition orderPosition) // obracowac klase dla view modeli
        {
            var orderPositionVM = new OrderPositionViewModel
            {
                Id = orderPosition.Id,
                OrderId = orderPosition.OrderId,
                ProductId = orderPosition.ProductId,
                UserId = orderPosition.UserId,
                Order = orderPosition.Order,
                Product = orderPosition.Product,
                User = orderPosition.User,
            };

            return orderPositionVM;
        }
    }
}
