using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            Products = new Collection<Product>(); //1  kategoria moze byc taka sama dla wielu produktów 
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
