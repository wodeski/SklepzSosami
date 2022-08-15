using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Converter;
using Serwis.Converters;
using Serwis.Core.Service;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.ViewModels;
using Serwis.Repository;
using System.Diagnostics;

namespace Serwis.Controllers
{
    public class AdminController : Controller
    {
        private readonly EmailSender _emailSender;
        private readonly ILogger<AdminController> _logger;
        private readonly IService _service;
        private readonly IWebHostEnvironment _hostEnviroment;

        [BindProperty]
        public ProductViewModel Product { get; set; }

        public AdminController(ILogger<AdminController> logger, IService service, IWebHostEnvironment hostEnviroment, EmailSender emailSender)
        {
            _logger = logger;
            _service = service;
            _hostEnviroment = hostEnviroment;
            _emailSender = emailSender;
           
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Service()
        {
            var products = await _service.GetProductsAsync();

            if (products == null)
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
            var productCategories = await _service.GetListOfProductCategories();
            if (id == 0)
            {
                Product = new();
                var productVManonym = Product;
                productVManonym.CategoriesList = productCategories;

                return View(productVManonym);
            }
            var product = await _service.GetProductAsync(id);

            var productVM = product.PrepareProductViewModel();
            productVM.CategoriesList = productCategories;

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductViewModel productVM)
        {
            //var errors = ModelState
            //    .Where(x => x.Value.Errors.Count > 0)
            //    .Select(x => new { x.Key, x.Value.Errors })
            //    .ToArray();
            if (!ModelState.IsValid)
                return View(productVM);

            var product = productVM.ConvertToProduct();

            if (product.Id == 0)
            {
                product.CreatedDate = DateTime.Now;
                AddImageToDirectory(product);
                await _service.CreateProductAsync(product);
            }
            else
            {
                await _service.UpdateProductAsync(product);
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
                await _service.DeleteProductAsync(id);
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
            var isCategoryListEmpty = await _service.IsProductCategoryListEmpty();
            if (isCategoryListEmpty)
            {
                await _service.CreateListOfCategories();
            }
            var products = await _service.GetProductsAsync();
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