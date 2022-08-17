using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Converter;
using Serwis.Converters;
using Serwis.Core.Service;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.Extensions;
using Serwis.Models.ViewModels;
using Serwis.Repository;
using System.Text.Json;

namespace Serwis.ShopControllers
{
    public class ShopController : Controller
    {
        private readonly IService _service;
        private EmailSender _emailSender;
        private List<OrderPositionViewModel> _orderPositions;
        private const int AnonymousId = 10;
        private const string AnonymousCart = "AnonyomousCart";
        private const string OrderSession = "OrderSession";
        public const string Id = "Id";

        public ShopController(IService service, EmailSender emailSender, List<OrderPositionViewModel> orderPositions)
        {
            _service = service;
            _emailSender = emailSender;
            _orderPositions = orderPositions;
        }

        [HttpPost]
        public async Task<IActionResult> Cart(int orderId, int[] quantityOfPositions) // tu przekaz ilosc prawdopodobnie w liscie 
        {
            HttpContext.Session.Remove(OrderSession); // usuwanie sesji z zamówieniem


            if (!quantityOfPositionsIsValid(quantityOfPositions))
            {
                return Json(new { success = false, message = "Wprowadzono nieprawidłowe dane!" });
            }
            try
            {
                var userId = HttpContext.Session.GetString("Id");
                if (userId == null)
                    userId = "10";
                var orderVM = await _service.SetOrderForPosition(Convert.ToInt32(userId), orderId, quantityOfPositions);

                var test = _service.SetOrderPositions(orderId, quantityOfPositions);

                TempData["test"] = JsonSerializer.Serialize<OrderViewModel>(orderVM);
                return Json(new { success = true, redirectToUrl = @Url.Action("OrderSummary", "Shop") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool quantityOfPositionsIsValid(int[] quantityOfPositions)
        {
            foreach (var position in quantityOfPositions)
            {
                if (position <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<IActionResult> Cart(int userId)
        {

            if (userId == 0)
            {
                //anonimowy uzytkownik
                var data = HttpContext.Session.GetComplexData<List<OrderPositionViewModel>>(AnonymousCart);
                return View(data);
            }
            var orderPositions = await _service.GetOrderPositionsForUserAsync(userId);//usuwa wszystkue te ktore mg byc false i byc ich wiecej kak jedno

            var orderPositionsVm = orderPositions.OrderPositionsIEnumerableToList(); // tu jest błąd
            return View(orderPositionsVm);
        }

        public async Task<IActionResult> SingleProductPurchase(int productId)//przetestowac zachowanie w momencie gdy zostanie podany string
        {
            if (productId <= 0) // proba wejscia na strone bez wybranego produktu
            {
                return RedirectToAction("Index", "Admin");
            }
            var product = await _service.GetProductAsync(productId);
            var productVM = product.PrepareProductViewModel();
            return View(productVM);
        }

        //[HttpPost]
        //public async Task<IActionResult> SingleProductPurchase(int productId, string priceValue)//do zmiany nazwa
        //{
        //    var product = await _service.GetProductAsync(productId);
        //    var productVM = product.PrepareProductViewModel();
        //    var userId = HttpContext.Session.GetString("Id");
        //    if (userId == null)
        //        userId = "7";
        //    var orderVM = SetOrderForProduct(Convert.ToInt32(userId), Convert.ToDecimal(priceValue));
        //    return Json(new { success = true, redirectToUrl = Url.Action("OrderSummary", "Shop") });//, orderVM) }); // działa!
        //}


        public async Task<IActionResult> OrderSummary()
        { // zabezpieczenie przed nullem
            var test = HttpContext.Session.GetComplexData<OrderViewModel>(OrderSession);
            if(test != null)
            {
                return View(test);
            }
            var orderVM = JsonSerializer.Deserialize<OrderViewModel>(TempData["Test"].ToString());
            HttpContext.Session.SetComplexData("OrderSession", orderVM);
            orderVM = HttpContext.Session.GetComplexData<OrderViewModel>(OrderSession);

            return View(orderVM); //stworzyc viemodel dla order position
        }

        //[HttpPost]
        //public async Task<IActionResult> OrderSummary(int productId, int userId, int orderId, string email)
        //{

        //    //sprawdzenie czy taki produtk taki uzytkownik i takie zamówienie istenija nastepnie 
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        var orderPositionVM = PrepareViewModelForInvoice(productId, userId, orderId);
        //        return View(orderPositionVM);
        //    }

        //    await _service.CreateOrderPositionAsync(productId, userId, orderId);

        //    await PayForOrderFromCart(orderId, userId, email);///?????????????

        //    return RedirectToAction("Index", "Admin");
        //}

        //private async Task<OrderPositionViewModel> PrepareViewModelForInvoice(int productId, int userId, int orderId)
        //{
        //    var order = await _service.FindOrderByIdAsync(orderId);

        //    var user = await _service.FindUserByIdAsync(userId);

        //    var product = await _service.FindProductByIdAsync(productId);

        //    var orderPositionVM = SetOrderPositionViewModelForInvoice(order, user, product);

        //    return orderPositionVM;
        //}

        public async Task<IActionResult> DeleteUserOrderPositionFromCart(string productId, int orderId)
        {
            var userId = HttpContext.Session.GetString(Id);

            if (userId == null)
            {
                await RemoveAnonymousPositionFromCart(Convert.ToInt32(productId));
                return Json(new { Success = true });
            }

            await _service.DeleteOrderPositionAsync(orderId, Convert.ToInt32(userId), Convert.ToInt32(productId));

            var listOfOrderPositions = await _service.CountOrderPositions(orderId, userId);

            if (listOfOrderPositions == 0) // usun zamowienie jesli nie ma w nim zadnych pozycji
            {
                await _service.DeleteOrder(Convert.ToInt32(userId), orderId);

            }

            return Json(new { Success = true });

        }

        //public async Task<IActionResult> OrderSummary(int productId, int userId)
        //{
        //    // na podstawie produktu id oraz id uzytkownika mozna by pobrac
        //    // id order poniewaz uzytkownikk moze miec tylko jedno zamowienie
        //    // niezrealizowane wiec mozna zakladac z gory ze wszystkie pozycje
        //    // naleza do tego samego zamowienia jedyne co wymaga korekty to przeniesienie
        //    // wartosci łacznej zamówienia
        //    //najwiecej sensu bedzie chyba mialo podanie order
        //    if (productId == 0)
        //        return RedirectToAction("Index", "Admin");

        //    if (userId == 0)
        //        userId = AnonymousId; ///!!! zmienić przy najblizszej okazji

        //    var product = await _service.GetProductAsync(productId); 

        //    var order = await SetOrderForProduct(userId); //ustawienie danych dla zamownia 

        //    var user = await _service.FindUserByIdAsync(userId);

        //    //od  tego momentu mozna łaczyc 
        //    var orderPositionVM = SetOrderPositionViewModelForProduct(product, order.ConvertToOrder(), user);

        //    return View(orderPositionVM); //stworzyc viemodel dla order position
        // }
        //private OrderPositionViewModel SetOrderPositionViewModelForInvoice(Order order, ApplicationUser user, Product product)
        //{
        //    return new OrderPositionViewModel
        //    {
        //        Order = order,
        //        User = user,
        //        Product = product,

        //    };
        //}

        //private OrderPositionViewModel SetOrderPositionViewModelForProduct(Product product, Order order, ApplicationUser user)
        //{
        //    var orderPositionVM = new OrderPositionViewModel
        //    {
        //        Product = product,
        //        User = user,
        //        Order = order
        //    };
        //    return orderPositionVM;
        //}










        //        [HttpGet]


        public async Task<IActionResult> Product(int productId)
        {
            var product = await _service.GetProductAsync(productId);

            var productVM = product.PrepareProductViewModel();

            return View(productVM);
        }

        [HttpPost]
        /////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        public async Task<ActionResult> PayForOrderFromCart(int orderId, int userId, string userEmail)
        {

            if (string.IsNullOrEmpty(userEmail))
                userEmail = HttpContext.Session.GetString("Email");

            var order = await _service.GetOrderPositionsForUserAsync(orderId, userId); // zwróci liste pozycji wraz z produktami dl konkretnego uzytkownika
            _emailSender.SendMail(userEmail, order);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<ActionResult> AddPositionToCart(int productId, string userId)
        {
            if (userId == null) // jesli uzytkownik jest niezalogowany 
                return await AddAnonymousPositionToCart(productId);
            // else
            //   return Json(new { Success = false, message = "Wystąpił błąd przy identyfikacji uzytkownika" });

            try
            {
                await _service.AddOrderPositionAsync(Convert.ToInt32(userId), productId);
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });
        }

        private async Task<ActionResult> AddAnonymousPositionToCart(int productId)
        {
            var data = _orderPositions;
            var id = data.Count();
            id++;
            var produkt = await _service.GetProductAsync(productId);
            var obj = new OrderPositionViewModel
            {
                Id = id,
                OrderId = 1,
                Product = produkt,
                Quantity = 1
            };

            data.Add(obj);
            HttpContext.Session.SetComplexData(AnonymousCart, data);
            return Json(new { Success = true });
        }

        private async Task<ActionResult> RemoveAnonymousPositionFromCart(int productId)
        {
            var data = _orderPositions;

            var produkt = await _service.GetProductAsync(productId);

            var positionToDelete = data.Find(x => x.Product.Id == productId);

            data.Remove(positionToDelete);
            HttpContext.Session.SetComplexData(AnonymousCart, data);
            return Json(new { Success = true });
        }


    }
}
