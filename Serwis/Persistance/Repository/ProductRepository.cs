using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _serviceDbContext;
        private List<Product> _defaultProducts;
        public ProductRepository(IApplicationDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
            _defaultProducts = new()
            {
                new Product
                {
                    CategoryId = 1,
                    ImageFileName = "cholula-min.jpg",
                    CreatedDate = DateTime.Now,
                    Description = "Cholula Hot Sauce jest produkowany" +
                    " od trzech pokoleń przez tą samą rodzinę z miasta" +
                    " Jalisco w Meksyku. Pilnie strzeżony przepis ma już" +
                    " ponad 100 lat. Nazwa sosu jest jednocześnie nazwą" +
                    " miasta w Meksyku, liczącego ok. 2.500 lat.",
                    Name = "Cholula Chili Lime Hot Sauce",
                    Price = 45.00M
                },
                new Product
                {
                    CategoryId = 2,
                    ImageFileName = "dzikibill-min.jpg",
                    CreatedDate = DateTime.Now,
                    Description = "Ninja jest obecnie najostrzejszym sosem" +
                    " w ofercie Dzikiego Billa. Jak prawdziwy Ninja pojawia" +
                    " się i znika – zależnie od dostępności potrzebnych do" +
                    " niego zabójczo ostrych papryczek Carolina Reaper.",
                    Name = "Dziki Bill Ninja",
                    Price = 60.00M
                },
                new Product
                {
                    CategoryId = 2,
                    ImageFileName = "cajohns-min.jpg",
                    CreatedDate = DateTime.Now,
                    Description = "Zdaniem producenta to jeden z najostrzejszyc" +
                    "h sosów Chipotle na rynku. Słodki wędzony posmak chili" +
                    " Chipotle i ostrą nutą.",
                    Name="CaJohns Killer Chipotle Hot Sauce",
                    Price = 37.00M
                },
                new Product
                {
                    CategoryId = 3,
                    ImageFileName = "monkey-min.jpg",
                    CreatedDate = DateTime.Now,
                    Description = "Sos w stylu tajskim, pikantny, ale też" +
                    " bardzo aromatyczny. Zdobywca nagrody \"Golden Chile Winner\" w 2009 roku.",
                    Name="Captain Tom Thai Monkey",
                    Price = 37.00M
                },
                new Product
                {
                    CategoryId = 4,
                    ImageFileName = "maddog-min.jpg",
                    CreatedDate = DateTime.Now,
                    Description = " Wykonane z czystej mieszanki chili i octu to doskonały" +
                    " sposób dodawania ulubionych chili do potraw bez dodatku" +
                    " innych zbędnych składników, które mogą przyćmić odświeżająco " +
                    "owocowy aromat chili Naga Morich.",
                    Name = "357 Mad Dog Naga Morich Puree",
                    Price = 58.00M
                },
                new Product
                {
                    CategoryId = 5,
                    ImageFileName = "assreaper-min.jpg",
                    CreatedDate = DateTime.Now,
                    Description = "Opakowanie to jednak " +
                    "dopiero zapowiedź porażenia, jakie może zaserwować sos." +
                    " Idealny pomysł na prezent, a także obowiązkowa pozycja" +
                    " w kolekcji każdego prawdziwego Chilihead’a.",
                    Name = "Ass Reaper Hot Sauce",
                    Price = 45.00M
                },

            };
        }

        public async Task<bool> DbHasAnyProducts()
        {
           var numberOfProducts = await _serviceDbContext.Products.CountAsync();
            if(numberOfProducts > 0)
            {
                return true;
            }
            return false;
        }
        public async Task InsertProducts()
        {
            foreach (var products in _defaultProducts)
            {
              await _serviceDbContext.Products.AddAsync(products);
            }
        }
        public async Task CreateProductAsync(Product product)
        {
            await _serviceDbContext.Products.AddAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var findItem = await _serviceDbContext.Products.SingleAsync(o => o.Id == id);
            _serviceDbContext.Products.Remove(findItem);
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _serviceDbContext.Products.ToListAsync(); //entity framework 
        }
        public async Task UpdateProductAsync(Product product)
        {
            var findItem = await _serviceDbContext.Products.SingleAsync(o => o.Id == product.Id);
            findItem.Name = product.Name;
            findItem.ImageFileName = product.ImageFileName;
            findItem.Description = product.Description;
            findItem.CategoryId = product.CategoryId;
            findItem.Price = product.Price;
            _serviceDbContext.Products.Update(findItem);
        }
        public async Task<Product> GetProductAsync(int id)
        {
            var find = await _serviceDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            return find;
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

        public async Task<decimal> CheckSumValueOfProducts(List<int> productId, List<int> quantity)
        {
            decimal sum = 0;
            for (int i = 0; i < productId.Count; i++)
            {
                var produkt = await _serviceDbContext.Products.SingleAsync(x => x.Id == productId[i]);
                sum = sum + (produkt.Price * quantity[i]);
            }
            return sum;

        }

        public IEnumerable<Product> Get(int categoryId = 0, string? name = null)
        {
            IQueryable<Product> products;
            products = _serviceDbContext.Products
          .Include(x => x.Categories);

            if (categoryId != 0)
            {
                products = products.Where(x => x.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(x => x.Name.Contains(name));
            }


            var productsToDisplay = products.OrderBy(x => x.Name).ToList();

            return productsToDisplay;
        }
    }
}
