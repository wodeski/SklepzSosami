using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.Extensions;
using Serwis.Repository;

namespace Serwis.ShopControllers
{
    public class ShopController : Controller
    {
        private readonly IRepository _repository;
        private EmailSender _emailSender;
        private List<OrderPosition> _orderPositions;

        public ShopController(IRepository repository, EmailSender emailSender, List<OrderPosition> orderPositions)
        {
            _repository = repository;
            _emailSender = emailSender;
            _orderPositions = orderPositions;
        }

        public IActionResult BuySingleProduct(int productId)
        {
            var product = _repository.GetProduct(productId);
            return View(product);
        }
        public IActionResult Cart(int userId)
        {

            if(userId == 0)
            {
                var data = HttpContext.Session.GetComplexData<List<OrderPosition>>("AnoniomousCart");
                return View(data);
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
        public ActionResult PayForOrderFromCart(int orderId, int userId)
        {
            var userEmail = HttpContext.Session.GetString("Email");

            var order = _repository.GetOrderPositionsForUser(orderId, userId); // zwróci liste pozycji wraz z produktami dl konkretnego uzytkownika
            _emailSender.SendMail(userEmail, order);


            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult AddPositionToCart(int productId, string userName) // id produktu
        {

            //wprowadzic walidacje jak uzytkownik nie jest zalogowany

            //co sie stanie jesli dodac do koszyka a nastepnie sie zaloguje9

            if (userName == null)
            {
                var data = _orderPositions;
                var id = data.Count();
                id++;
                var produkt = _repository.GetProduct(productId);
                var obj = new OrderPosition
                {
                    Id = id,
                    OrderId = 1,
                    Product = produkt,
                    
                    
                };

                data.Add(obj);
                HttpContext.Session.SetComplexData("AnoniomousCart", data);
                    return Json(new { Success = true });
            }

            
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
