using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Converter;
using Serwis.Converters;
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
        public ProductViewModel Product { get; set; }

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
            var products = await _irepository.GetProductsAsync();

            if (products.ToList() == null)
            {
                var emptyList = new List<ProductViewModel>();
                return View(emptyList);
            }
            var productsVM = products.ProductsIEnumerableToList();

            return View(productsVM);

        }

        [Authorize(Policy ="AdminOnly")]
        public async Task<ActionResult> Upsert(int id = 0)
        {
            var productCategories = await _irepository.GetListOfProductCategories();
            if (id == 0)
            {
                Product = new();
                var productVManonym = Product;
                productVManonym.CategoriesList = productCategories;

                return View(productVManonym);
            }
            var product = await _irepository.GetProductAsync(id);

            var productVM = product.PrepareProductViewModel();
            productVM.CategoriesList = productCategories;

            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Upsert(ProductViewModel productVM)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
            if (!ModelState.IsValid)
                return View(productVM);

            var product = productVM.ConvertToProduct();

            if (product.Id == 0)
            {
                product.CreatedDate = DateTime.Now;
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
                await _irepository.DeleteProductAsync(id);
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
            var isCategoryListEmpty = await _irepository.IsProductCategoryListEmpty();
            if (isCategoryListEmpty)
            {
                await _irepository.CreateListOfCategories();
            }
            var products = await _irepository.GetProductsAsync();
            if (products.Count() == 0)
            { 
                var emptyListOfProducts = new List<ProductViewModel>();
                return View(emptyListOfProducts);
            }

            var productsVM = products.ProductsIEnumerableToList();

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
    }
}