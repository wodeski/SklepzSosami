using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Repository;

namespace Serwis.ShopControllers
{
    public class ShopController : Controller
    {
        private readonly IRepository _repository;

        public ShopController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Cart(int userId)
        {

            if(userId == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //zapytanie skonstruować tak aby wyswietlalo sie dla konkretnego uzytkownika
           var positions =  _repository.GetPositionsForUser(userId);
            return View(positions);
        }

        

        public IActionResult Product(int productId)
        {
            var product = _repository.GetProduct(productId);
            return View(product);
        }


        
        public IActionResult Buy(int orderId)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Add(int id, string userName) // id produktu
        {
            //wprowadzic walidacje jak uzytkownik nie jest zalogowany
            try
            {
                var find = _repository.FindUser(userName);

                if(find == null)//nie dziala jak powinno
                {
                    TempData["Error"] = "Nie udało się dodać";
                    return RedirectToAction("Index", "Home");
                }

                _repository.AddPosition(find.Id, id);

            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            TempData["Error"] = "Dodano do koszyka!";
            return Json(new { Success = true });
        }
    }
}
