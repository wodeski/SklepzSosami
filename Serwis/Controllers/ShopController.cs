using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.Extensions;
using Serwis.Models.ViewModels;
using Serwis.Repository;

namespace Serwis.ShopControllers
{
    public class ShopController : Controller
    {
        private readonly IRepository _repository;
        private EmailSender _emailSender;
        private List<OrderPositionViewModel> _orderPositions;

        public ShopController(IRepository repository, EmailSender emailSender, List<OrderPositionViewModel> orderPositions)
        {
            _repository = repository;
            _emailSender = emailSender;
            _orderPositions = orderPositions;

        }

        public IActionResult BuySingleProduct(int productId)
        {
            var product = _repository.GetProduct(productId);
            var productVM = PrepareProductViewModel(product);
            return View(productVM);
        }

        //[HttpPost]
        //public IActionResult BuySingleProduct()
        //{
        //    return View()
        //}

        [HttpPost]
        public IActionResult InvoiceOfOrder(int productId, int userId, int orderId, string email)
        {
            //sprawdzenie czy taki produtk taki uzytkownik i takie zamówienie istenija nastepnie 
            if (string.IsNullOrEmpty(email))
            {
                var order = _repository.FindOrderById(orderId);
                var user = _repository.FindUserById(userId);
                var product = _repository.FindProductById(productId);

                var orderPositionVM = new OrderPositionViewModel
                {
                    Order=order,
                    User=user,
                    Product=product,

                };
                TempData["EmailValidationError"] = "Podaj maila!!!";
                return View(orderPositionVM);
            }
            PayForOrderFromCart(orderId, userId, email);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult InvoiceOfOrder(int productId, int userId)
        {
            if (productId == 0)
                return RedirectToAction("Index", "Home");

            if (userId == 0)
                userId = 3; ///!!! zmienić przy najblizszej okazji

            var product = _repository.GetProduct(productId); //async dac
            var order = SetOrderForProduct(userId); //ustawienie danych dla zamownia 
            var user = _repository.FindUserById(userId);
            var orderPositionVM = SetOrderPositionViewModelForProduct(product, order, user);

            return View(orderPositionVM); //stworzyc viemodel dla order position
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

        private Order SetOrderForProduct(int userId)
        {
            var order = new Order
            {
                UserId = userId,
                IsCompleted = false,
                Title = $"Fak/{userId}/{DateTime.Now.ToString("dd-MM-yyyy:mm:ss")}"
            };
            _repository.CreateOrder(order);
            return order;
        }

        private ProductViewModel PrepareProductViewModel(Product product) // obracowac klase dla view modeli
        {
            var productVM = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                ImageFileName = product.ImageFileName,
                Price = product.Price,
                OrderPositions = product.OrderPositions
            };

            return productVM;
        }

        private ApplicationUserViewModel PrepareApplicationUserViewModel(ApplicationUser user) // obracowac klase dla view modeli
        {
            var userVM = new ApplicationUserViewModel
            {
                Id = user.Id,
                Password = user.Password,
                UserName = user.UserName,
                CreatedDate = user.CreatedDate,
                Email = user.Email,
                Orders = user.Orders,
                
                
            };

            return userVM;
        }
        private OrderViewModel PrepareOrderViewModel(Order order) // obracowac klase dla view modeli
        {
            var orderVM = new OrderViewModel
            {
                Id = order.Id,
                Title = order.Title,
                UserId = order.UserId,
                OrderPositions = order.OrderPositions,
            };

            return orderVM;
        }
        private OrderPositionViewModel PrepareOrderPositionViewModel(OrderPosition orderPosition) // obracowac klase dla view modeli
        {
            var orderPositionVM = new OrderPositionViewModel
            {
                Id= orderPosition.Id,
                OrderId = orderPosition.OrderId,
                ProductId = orderPosition.ProductId,
                UserId = orderPosition.UserId,
                Product = orderPosition.Product,
                Order = orderPosition.Order,
                User = orderPosition.User

            };

            return orderPositionVM;
        }

        public IActionResult Cart(int userId)
        {

            if(userId == 0)
            {
                var data = HttpContext.Session.GetComplexData<List<OrderPositionViewModel>>("AnoniomousCart");
                return View(data);
            }
            //zapytanie skonstruować tak aby wyswietlalo sie dla konkretnego uzytkownika
           var orderPositions = _repository.GetPositionsForUser(userId);
            //tutututututututuut

            var orderPositionsVm = OrderPositionsIEnumerableToList(orderPositions);
            return View(orderPositionsVm);
        }

        private List<OrderPositionViewModel> OrderPositionsIEnumerableToList(IEnumerable<OrderPosition> orderPositions)
        {
            var orderPositionVM = new List<OrderPositionViewModel>();
            foreach (var orderPosition in orderPositions)
            {
                orderPositionVM.Add(
                    new OrderPositionViewModel
                    {
                       Order = orderPosition.Order,
                       Product = orderPosition.Product,
                    });
            }

            return orderPositionVM;

        }

        public IActionResult Product(int productId)
        {
            var product = _repository.GetProduct(productId);

            var productVM = PrepareProductViewModel(product);

            return View(productVM);
        }


        [HttpPost]
        public ActionResult PayForOrderFromCart(int orderId, int userId, string userEmail)
        {
            if(string.IsNullOrEmpty(userEmail))
                userEmail = HttpContext.Session.GetString("Email");

            var order = _repository.GetOrderPositionsForUser(orderId, userId); // zwróci liste pozycji wraz z produktami dl konkretnego uzytkownika
            _emailSender.SendMail(userEmail, order);


            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult AddPositionToCart(int productId, string userName) // id produktu
        {
            if (userName == null)
                return SetAnonimousCart(productId);

            try
            {
                var find = _repository.FindUser(userName);

                if(find == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                _repository.AddPosition(find.Id, productId);

            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });
        }

        private ActionResult SetAnonimousCart(int productId)
        {
            var data = _orderPositions;
            var id = data.Count();
            id++;
            var produkt = _repository.GetProduct(productId);
            var obj = new OrderPositionViewModel
            {
                Id = id,
                OrderId = 1,
                Product = produkt,


            };

            data.Add(obj);
            HttpContext.Session.SetComplexData("AnoniomousCart", data);
            return Json(new { Success = true });
        }
    }
}
