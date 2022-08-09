using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.ViewModels;
using Serwis.Repository;
using System.Diagnostics;

namespace Serwis.Controllers
{



    public class HomeController : Controller
    {
        private readonly EmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository _irepository;
        private readonly IWebHostEnvironment _hostEnviroment;

        [BindProperty]
        public Product Product { get; set; }

        //[BindProperty]
        //public Credential Credential { get; set; } //w innym kontrolerze to trzeba umiescic

        public HomeController(ILogger<HomeController> logger, IRepository irepository, IWebHostEnvironment hostEnviroment, EmailSender emailSender)
        {
            _logger = logger;
            _irepository = irepository;
            _hostEnviroment = hostEnviroment;
            _emailSender = emailSender;
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Service()
        {

            var products = await _irepository.GetItemsAsync();
            if (products.ToList() == null)
            {
                var emptyList = new List<ProductViewModel>();
                return View(emptyList);
            }
            var productsVM = ProductsIEnumerableToList(products);

            return View(productsVM);

        }

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

        //public IActionResult Login(Credential credential)
        //{
        //    return View(credential);

        //}

        [AllowAnonymous]
        public async Task<ActionResult> Upsert(int id = 0)
        {
            if (id == 0)
            {
                Product = new();
                var productVManonim = PrepareProductViewModel(Product);
                return View(productVManonim);
            }
            var item = await _irepository.GetItemAsync(id);

            var productVM = PrepareProductViewModel(item);

            return View(productVM);
        }


        [HttpPost]

        public async Task<IActionResult> Upsert(Product product)
        {

            //var errors = ModelState
            //    .Where(x => x.Value.Errors.Count > 0)
            //    .Select(x => new { x.Key, x.Value.Errors })
            //    .ToArray();
            if (!ModelState.IsValid)
                return View(product);

            if (product.Id == 0)
            {
                AddImageToDirectory(product);
                await _irepository.CreateProductAsync(product);
            }
            else
            {
                await _irepository.UpdateProductAsync(product);
            }
            return RedirectToAction("Service");
        }

        private void AddImageToDirectory(Product product)
        {
            var imageFilePath = AddPathToImage(product);
            using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
            {
                product.ImageFile.CopyTo(fileStream);
            }
        }

        private string AddPathToImage(Product product)
        {
            var wwwRootPath = _hostEnviroment.WebRootPath;
            var fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
            var extension = Path.GetExtension(product.ImageFile.FileName);
            product.ImageFileName = fileName + DateTime.Now.ToString("yyMMddmmss") + extension;
            var imageFilePath = Path.Combine(wwwRootPath + "/Images/", product.ImageFileName);
            return imageFilePath;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _irepository.DeleteItemAsync(id);
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = true });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _irepository.GetItemsAsync();
            if (products.Count() == 0)
            { // tu raczej nalezy sprawdzic czy lista jest pusta a nie null
                var emptyListOfProducts = new List<ProductViewModel>();
                return View(emptyListOfProducts);
            }

            var productsVM = ProductsIEnumerableToList(products);

            return View(productsVM);
        }

        [Authorize(Policy = "User")] // jesli ma obaj maja miec mozliwosc wejscia trzeba dac roles
        public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            return View();
        }

        private ProductViewModel PrepareProductViewModel(Product product) // obracowac klase dla view modeli
        {
            var productVM = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                ImageFileName = product.ImageFileName,
                Price = product.Price,
                OrderPositions = product.OrderPositions
            };

            return productVM;
        }

        private ApplicationUserViewModel PrepareApplicationUserViewModel(ApplicationUser user) // obracowac klase dla view modeli
        {
            var userVM = new ApplicationUserViewModel
            {
                Id = user.Id,
                Password = user.Password,
                UserName = user.UserName,
                CreatedDate = user.CreatedDate,
                Email = user.Email,
                Orders = user.Orders,


            };

            return userVM;
        }
        private OrderViewModel PrepareOrderViewModel(Order order) // obracowac klase dla view modeli
        {
            var orderVM = new OrderViewModel
            {
                Id = order.Id,
                Title = order.Title,
                UserId = order.UserId,
                OrderPositions = order.OrderPositions,
            };

            return orderVM;
        }
        private OrderPositionViewModel PrepareOrderPositionViewModel(OrderPosition orderPosition) // obracowac klase dla view modeli
        {
            var orderPositionVM = new OrderPositionViewModel
            {
                Id = orderPosition.Id,
                OrderId = orderPosition.OrderId,
                ProductId = orderPosition.ProductId,
                UserId = orderPosition.UserId,
                Product = orderPosition.Product,
                Order = orderPosition.Order,
                User = orderPosition.User

            };

            return orderPositionVM;
        }

    }
}