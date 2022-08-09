using Serwis.Models.Domains;
using Serwis.Models.ViewModels;

namespace Serwis.Converters
{
    public class ListConverter
    {
        private List<ProductViewModel> ProductsIEnumerableToList(IEnumerable<Product> products)
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
        private List<OrderPositionViewModel> OrderPositionsIEnumerableToList(IEnumerable<OrderPosition> orderPositions)
        {
            var orderPositionVM = new List<OrderPositionViewModel>();
            foreach (var orderPosition in orderPositions)
            {
                orderPositionVM.Add(
                    new OrderPositionViewModel
                    {
                        Order = orderPosition.Order,
                        Product = orderPosition.Product,
                    });
            }

            return orderPositionVM;

        }
    }
}
