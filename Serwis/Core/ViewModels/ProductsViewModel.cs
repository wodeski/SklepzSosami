using Serwis.Models.Domains;
using Serwis.Persistance;

namespace Serwis.Core.ViewModels
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public FilterProducts FilterProducts { get; set; }
    }
}
