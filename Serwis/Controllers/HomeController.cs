using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Repository;
using System.Diagnostics;

namespace Serwis.Controllers
{
    
   

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository _irepository;
        private readonly IWebHostEnvironment _hostEnviroment;

        [BindProperty]
        public Product Product { get; set; }

        //[BindProperty]
        //public Credential Credential { get; set; } //w innym kontrolerze to trzeba umiescic

        public HomeController(ILogger<HomeController> logger, IRepository irepository, IWebHostEnvironment hostEnviroment)
        {
            _logger = logger;
            _irepository = irepository;
            _hostEnviroment = hostEnviroment;
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Service()
        {

            var items = await _irepository.GetItemsAsync();
            if (items == null)
            {
                var emptyList = new List<Product>();
                return View(emptyList);
            }
            return View(items);

        }

        //public IActionResult Login(Credential credential)
        //{
        //    return View(credential);

        //}

        [AllowAnonymous]
        public async Task<ActionResult> Upsert(int id = 0)
        {
            Product = new();
            if (id == 0)
            {
                return View(Product);
            }
            var item = await _irepository.GetItemAsync(id);
            return View(item);
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
            using(var fileStream = new FileStream(imageFilePath, FileMode.Create))
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


        //[HttpPost]
        //public async Task<IActionResult> Update(Order order)
        //{
        //    if (!ModelState.IsValid)
        //        return View(order);
        //    await _irepository.UpdateItemAsync(order);

        //    return RedirectToAction("Service");
        //}

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
            var items = await _irepository.GetItemsAsync();
            if (items == null)
            {
                var emptyList = new List<Order>();
                return View(emptyList);
            }
            return View(items);
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