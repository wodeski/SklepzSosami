using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Repository;

namespace Serwis.ShopControllers
{
    public class ShopController : Controller
    {
        private readonly IRepository _repository;
        private readonly ReportRepository _reportRepository;
        private EmailSender _emailSender;

        public ShopController(IRepository repository, ReportRepository reportRepository, EmailSender emailSender)
        {
            _repository = repository;
            _reportRepository = reportRepository;
            _emailSender = emailSender;
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


        [HttpPost]
        public ActionResult OrderPayment(int orderId, int userId)
        {
            var userEmail = HttpContext.Session.GetString("Email");

            var order = _repository.GetOrderPositionsForUser(orderId, userId); // zwróci liste pozycji wraz z produktami dl konkretnego uzytkownika
            _emailSender.SendMail(userEmail, order);


            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult Add(int productId, string userName) // id produktu
        {
            //wprowadzic walidacje jak uzytkownik nie jest zalogowany

            if(userName == null)
                return RedirectToAction("Index", "Home");
            
            try
            {
                var find = _repository.FindUser(userName);

                if(find == null)//nie dziala jak powinno
                {
                    TempData["Error"] = "Nie udało się dodać";
                    return RedirectToAction("Index", "Home");
                }

                _repository.AddPosition(find.Id, productId);

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
