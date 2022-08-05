using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Repository;

namespace Serwis.Controllers
{
    public class CartController : Controller
    {
        private readonly IRepository _repository;

        public CartController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Cart()
        {
           var positions =  _repository.GetPositions();
            return View(positions);
        }


        [HttpPost]
        public ActionResult Add(int id, string userName) // id produktu
        {
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
