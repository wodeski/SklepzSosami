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
        public Order Order { get; set; }

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
                return View();
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
            Order = new();
            if (id == 0)
            {
                return View(Order);
            }
            var item = await _irepository.GetItemAsync(id);
            return View(item);
        }


        [HttpPost]

        public async Task<IActionResult> Upsert(Order order)
        {
            //var errors = ModelState
            //    .Where(x => x.Value.Errors.Count > 0)
            //    .Select(x => new { x.Key, x.Value.Errors })
            //    .ToArray();
            if (!ModelState.IsValid)
                return View(order);
            
            if (order.Id == 0)
            {
                AddImageToDirectory(order);
                await _irepository.CreateItemAsync(order);
            }
            else 
            {
            await _irepository.UpdateItemAsync(order);
            }
            return RedirectToAction("Service");
        }

        private void AddImageToDirectory(Order order)
        {
            var imageFilePath = AddPathToImage(order);
            using(var fileStream = new FileStream(imageFilePath, FileMode.Create))
            {
                order.ImageFile.CopyTo(fileStream);
            }
        }

        private string AddPathToImage(Order order)
        {
            var wwwRootPath = _hostEnviroment.WebRootPath;
            var fileName = Path.GetFileNameWithoutExtension(order.ImageFile.FileName);
            var extension = Path.GetExtension(order.ImageFile.FileName);
            order.ImageFileName = fileName + DateTime.Now.ToString("yyMMddmmss") + extension;
            var imageFilePath = Path.Combine(wwwRootPath + "/Images/", order.ImageFileName);
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
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });
        }

        [AllowAnonymous]
        public IActionResult Index()
        {

            return View();
        }

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