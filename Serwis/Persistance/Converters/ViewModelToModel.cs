using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Converter
{
    public static class ViewModelToModel
    {
        
        public static Product ConvertToProduct(this ProductViewModel productVM) // obracowac klase dla view modeli
        {
            var product = new Product
            {
                Id = productVM.Id,
                Name = productVM.Name,
                Description = productVM.Description,
                CreatedDate = productVM.CreatedDate,
                ImageFileName = productVM.ImageFileName,
                ImageFile = productVM.ImageFile,
                Price = productVM.Price,
                OrderPositions = productVM.OrderPositions,
                CategoryId = productVM.CategoryId,
            };

            return product;
        }

        public static ApplicationUser ConvertToApplicationUserFromRegisterViewModel(this RegisterViewModel registerVM) // obracowac klase dla view modeli
        {
            var user = new ApplicationUser
            {
                //moze sie srac o brak id
                Password = registerVM.Password,
                UserName = registerVM.UserName,
                CreatedDate = registerVM.CreatedDate,
                Email = registerVM.Email,


            };

            return user;
        }
        public static ApplicationUser ConvertToApplicationUser(this ApplicationUserViewModel userVM) // obracowac klase dla view modeli
        {
            var user = new ApplicationUser
            {
                Id = userVM.Id,
                Password = userVM.Password,
                UserName = userVM.UserName,
                CreatedDate = userVM.CreatedDate,
                Email = userVM.Email,
                Orders = userVM.Orders


            };

            return user;
        }
        public static Order  ConvertToOrder(this OrderViewModel orderVM) // obracowac klase dla view modeli
        {
            var order = new Order
            {
                Id = orderVM.Id,
                Title = orderVM.Title,
                UserId = orderVM.UserId,
                OrderPositions = orderVM.OrderPositions
            };

            return order;
        }
        public static OrderPosition  ConvertToOrderPosition(this OrderPositionViewModel orderPositionVM) // obracowac klase dla view modeli
        {
            var orderPosition = new OrderPosition
            {
                Id = orderPositionVM.Id,
                OrderId = orderPositionVM.OrderId,
                ProductId = orderPositionVM.ProductId,
                UserId = orderPositionVM.UserId,
                Order = orderPositionVM.Order,
                Product = orderPositionVM.Product,
                User = orderPositionVM.User,
            };

            return orderPosition;
        }

    }
}
