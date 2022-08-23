using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Converters
{
    public static class ListConverter
    {
        public static List<ProductViewModel> ProductsIEnumerableToList(this IEnumerable<Product> products)
        {
            var productsVM = new List<ProductViewModel>();
            foreach (var product in products)
            {
                productsVM.Add(
                    new ProductViewModel
                    {
                        Id = product.Id,
                        CreatedDate = product.CreatedDate,
                        Description = product.Description,
                        ImageFileName = product.ImageFileName,
                        Name = product.Name,
                        Price = product.Price,
                        OrderPositions = product.OrderPositions

                    });
            }

            return productsVM;
        }
        public static List<OrderPositionViewModel> OrderPositionsIEnumerableToList(this IEnumerable<OrderPosition> orderPositions)
        {
            var orderPositionVM = new List<OrderPositionViewModel>();
            foreach (var orderPosition in orderPositions)
            {
                orderPositionVM.Add(
                    new OrderPositionViewModel
                    {
                        Quantity = 1,
                        Order = orderPosition.Order,
                        Product = orderPosition.Product,
                        Id = orderPosition.Id,
                        OrderId = orderPosition.OrderId,
                        ProductId = orderPosition.ProductId
                    });
            }

            return orderPositionVM;

        }

        public static List<OrderViewModel> OrderIEnumerableToList(this IEnumerable<Order> orders)
        {
            var ordersVM = new List<OrderViewModel>();
            foreach (var position in orders)
            {
                ordersVM.Add(
                    new OrderViewModel
                    {
                        FullPrice = position.FullPrice,
                        OrderPositions = position.OrderPositions,
                        Title = position.Title,
                        UserId = position.UserId,
                        Id = position.Id
                    });
            }

            return ordersVM;

        }


    }
}
