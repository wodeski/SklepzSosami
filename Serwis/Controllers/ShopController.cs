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

namespace Serwis.ShopControllers
{
    public class ShopController : Controller
    {
        private readonly IService _service;
        private EmailSender _emailSender;
        private List<OrderPositionViewModel> _orderPositions;
        private const int AnonymousId = 7;
        private const string AnonymousCart = "AnonyomousCart";
        public const string Id = "Id";

        public ShopController(IService service, EmailSender emailSender, List<OrderPositionViewModel> orderPositions)
        {
            _service = service;
            _emailSender = emailSender;
            _orderPositions = orderPositions;
        }

        public async Task<IActionResult> BuySingleProduct(int productId)//do zmiany nazwa
        {
            var product = await _service.GetProductAsync(productId);
            var productVM = product.PrepareProductViewModel();
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> InvoiceOfOrder(int productId, int userId, int orderId, string email)
        {
            
            //sprawdzenie czy taki produtk taki uzytkownik i takie zamówienie istenija nastepnie 
            if (string.IsNullOrEmpty(email))
            {
                var orderPositionVM = PrepareViewModelForInvoice(productId, userId, orderId);
                return View(orderPositionVM);
            }
            var orderPosition = SetOrderPosition(productId, userId, orderId);

            await _service.CreateOrderPositionAsync(orderPosition);

            PayForOrderFromCart(orderId, userId, email);

            return RedirectToAction("Index", "Home");
        }

        private async Task<OrderPositionViewModel> PrepareViewModelForInvoice(int productId, int userId, int orderId)
        {
            var order = await _service.FindOrderByIdAsync(orderId);

            var user = await _service.FindUserByIdAsync(userId);

            var product = await _service.FindProductByIdAsync(productId);

            var orderPositionVM = SetOrderPositionViewModel(order, user, product);

            return orderPositionVM;
        }

        public async Task<IActionResult> DeleteUserOrderPositionFromCart(string productId, int orderId)
        {
            var userId = HttpContext.Session.GetString(Id);

            if (userId == null)
            {
                await RemoveAnonymousPositionFromCart(Convert.ToInt32(productId));
                return Json(new { Success = true });
            }

           await _service.DeleteOrderPositionAsync(orderId, Convert.ToInt32(userId), Convert.ToInt32(productId));
            return Json(new { Success = true });

        }

        public async Task<IActionResult> InvoiceOfOrder(int productId, int userId)
        {
            // na podstawie produktu id oraz id uzytkownika mozna by pobrac
            // id order poniewaz uzytkownikk moze miec tylko jedno zamowienie
            // nie zrealizowane wie cmozna zakladac z gory ze wszystkie pozycje
            // naleza do tego samego zamowienia jedyne co wymaga korekty to przeniesienie
            // wartosci łacznej zamówienia
            //najwiecej sensu bedzie chyba mialo podanie order
            if (productId == 0)
                return RedirectToAction("Index", "Home");

            if (userId == 0)
                userId = AnonymousId; ///!!! zmienić przy najblizszej okazji

            var product = await _service.GetProductAsync(productId); 

            var order = await SetOrderForProduct(userId); //ustawienie danych dla zamownia 

            var user = await _service.FindUserByIdAsync(userId);

            //od  tego momentu mozna łaczyc 
            var orderPositionVM = SetOrderPositionViewModelForProduct(product, order, user);

            return View(orderPositionVM); //stworzyc viemodel dla order position
        }
        private OrderPositionViewModel SetOrderPositionViewModel(Order order, ApplicationUser user, Product product)
        {
            return new OrderPositionViewModel
            {
                Order = order,
                User = user,
                Product = product,

            };
        }
        private OrderPosition SetOrderPosition(int productId, int userId, int orderId)
        {
            return new OrderPosition
            {
                OrderId = orderId,
                UserId = userId,
                ProductId = productId

            };
        }
        private OrderPositionViewModel SetOrderPositionViewModelForProduct(Product product, Order order, ApplicationUser user)
        {
            var orderPositionVM = new OrderPositionViewModel
            {
                Product = product,
                User = user,
                Order = order
            };
            return orderPositionVM;
        }

        private async Task<Order> SetOrderForProduct(int userId)
        {
            var order = new Order
            {
                UserId = userId,
                IsCompleted = false,
                Title = $"Fak/{userId}/{DateTime.Now.ToString("dd-MM-yyyy:mm:ss")}"
            };
           await _service.CreateOrderAsync(order);
            return order;
        }



        public async Task<IActionResult> Cart(int userId)
        {

            if (userId == 0)
            {
                //anonimowy uzytkownik
                var data = HttpContext.Session.GetComplexData<List<OrderPositionViewModel>>(AnonymousCart);
                return View(data);
            }
            var orderPositions = await _service.GetPositionsForUserAsync(userId);//usuwa wszystkue te ktore mg byc false i byc ich wiecej kak jedno

            var orderPositionsVm = orderPositions.OrderPositionsIEnumerableToList();
            return View(orderPositionsVm);
        }

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
            if (userId == null && !User.Identity.IsAuthenticated) // jesli uzytkownik jest niezalogowany 
            {
                return await AddAnonymousPositionToCart(productId);

            }
            else
            {
                return Json(new { Success = false, message = "Wystąpił błąd przy identyfikacji uzytkownika" });
            }

            try 
            {
                //AddOrderIfNotExist();
                await _service.AddPositionAsync(Convert.ToInt32(userId), productId);
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });
        }

        //private void AddOrderIfNotExist()
        //{
        //    //zakladamy ze moze byc tylko jedno zamowienie niezrealizowane
        //    //znalesc ilosc order aby utworzyc order id  
        //    if (!isThereAnyOrderIncomplete)// jesli nie ma zadnego nieukonczonego zamowienia to stworz nowe 
        //    {
        //        var newOrder = new Order
        //        {
        //            UserId = UserId, //dodanie nowego zamowienia dla usera
        //            IsCompleted = false
        //        };
        //        //dodac nowe zamowienie
        //        await _serviceDbContext.Orders.AddAsync(newOrder);
        //        await _serviceDbContext.SaveChangesAsync();
        //        orderId++;
        //    }
        //}

        private async Task<ActionResult> AddAnonymousPositionToCart(int productId)
        {
            var data = _orderPositions;
            var id = data.Count();
            id++;
            var produkt =  await _service.GetProductAsync(productId);
            var obj = new OrderPositionViewModel
            {
                Id = id,
                OrderId = 1,
                Product = produkt,
            };

            data.Add(obj);
            HttpContext.Session.SetComplexData(AnonymousCart, data);
            return Json(new { Success = true });
        }

        private async Task<ActionResult> RemoveAnonymousPositionFromCart(int productId)
        {
            var data = _orderPositions;
            
            var produkt = await _service.GetProductAsync(productId);
            
            var positionToDelete = data.Find(x=>x.Product.Id == productId);

            data.Remove(positionToDelete);
            HttpContext.Session.SetComplexData(AnonymousCart, data);
            return Json(new { Success = true });
        }
    }
}
