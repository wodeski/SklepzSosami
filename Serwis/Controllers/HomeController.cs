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

        [BindProperty]
        public Order Order { get; set; }

        public HomeController(ILogger<HomeController> logger, IRepository irepository)
        {
            _logger = logger;
            _irepository = irepository;
        }

        public async Task<IActionResult> Service()
        {
            var items = await _irepository.GetItemsAsync();
            if(items == null)
            {
                return View();
            }
            return View(items);

        }

        public IActionResult Login()
        {
            return View();

        }


        public async Task<ActionResult> Upsert(int id = 0)
        {
            Order = new ();
            if (id == 0)
                return View(Order);

            var item = await _irepository.GetItemAsync(id);
            return View(item);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Order order)
        {
            if (!ModelState.IsValid)
                return View(order);
            if (order.Id == 0)
                await _irepository.CreateItemAsync(order);
            await _irepository.UpdateItemAsync(order);

            return RedirectToAction("Service");
        }

        

        [HttpPost]
        public async Task<IActionResult> Update(Order order)
        {
            if (!ModelState.IsValid)
                return View(order);
            await _irepository.UpdateItemAsync(order);

            return RedirectToAction("Service");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await  _irepository.DeleteItemAsync(id);
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });
        }

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