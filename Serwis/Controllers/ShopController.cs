using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Converter;
using Serwis.Converters;
using Serwis.Core.Models;
using Serwis.Core.Service;
using Serwis.Core.ViewModels;
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
        private List<OrderPositionViewModel> _orderPositions;
        private const int AnonymousId = 10;
        private const string AnonymousCart = "AnonyomousCart";
        private const string OrderSession = "OrderSession";
        public const string Id = "Id";

        public ShopController(IService service, List<OrderPositionViewModel> orderPositions)
        {
            _service = service;
            _orderPositions = orderPositions;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ProductsViewModel productsViewModel = new();

            try
            {
                var products = await _service.GetProductsAsync();

                var categories = await _service.GetListOfProductCategories();
                 productsViewModel = new ProductsViewModel
                {
                    FilterProducts = new FilterProducts(),
                    Products = products,
                    Categories = categories
                };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return View(productsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Cart(int orderId, int[] quantityOfPositions) // tu przekaz ilosc prawdopodobnie w liscie 
        {
            HttpContext.Session.Remove(OrderSession); // usuwanie sesji z zamówieniem

            if (!quantityOfPositionsIsValid(quantityOfPositions)) // walidacja 
            {
                return Json(new { success = false, message = "Wprowadzono nieprawidłowe dane!" });
            }
            try
            {
                var orderVM = await _service.SetOrderForPosition(orderId, quantityOfPositions);

                //  var test = _service.SetOrderPositions(orderId, quantityOfPositions);

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

            if (product == null)
                return View((ProductViewModel)null);

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
        //    var orderVM = _service.SetOrderForPosition(Convert.ToInt32(userId), Convert.ToDecimal(priceValue));
        //    return Json(new { success = true, redirectToUrl = Url.Action("OrderSummary", "Shop") });//, orderVM) }); // działa!
        //}


        public async Task<IActionResult> OrderSummary()
        {
            var test = HttpContext.Session.GetComplexData<OrderViewModel>(OrderSession); // na wypadek gdyby ktos przeładował strone zabezpiecznie przed null 
            if (test != null)
            {
                return View(test);
            }
            var orderVM = JsonSerializer.Deserialize<OrderViewModel>(TempData["Test"].ToString());
            HttpContext.Session.SetComplexData("OrderSession", orderVM);
            orderVM = HttpContext.Session.GetComplexData<OrderViewModel>(OrderSession);

            return View(orderVM);
        }

        [HttpPost]
        public async Task<IActionResult> OrderSummary(string email)
        {
            var orderVM = HttpContext.Session.GetComplexData<OrderViewModel>(OrderSession);
            var order = orderVM.ConvertToOrder(); // order powinien byc guidem

            // poki co wysylanie maila dla anonima
            await _service.CreateOrderAsync(order); // zapisanie zamówienia do bazy zmienic na zapisywanie zamowienia gotowego

            try
            {
                foreach(var position in orderVM.OrderPositions)
                {
                    try
                    {
                        await _service.UpdateOrderPositionAsync(position, order);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }


                _service.SendMail(email, orderVM);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = true }); // zrobic przekierwoanie do strony głownej

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

            var listOfOrderPositions = await _service.CountOrderPositions(orderId, userId);

            if (listOfOrderPositions == 0) // usun zamowienie jesli nie ma w nim zadnych pozycji
            {
                await _service.DeleteOrder(Convert.ToInt32(userId), orderId);

            }

            return Json(new { Success = true });

        }
        private OrderPositionViewModel SetOrderPositionViewModelForInvoice(Order order, ApplicationUser user, Product product)
        {
            return new OrderPositionViewModel
            {
                Order = order,
                User = user,
                Product = product,

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










        //        [HttpGet]


        public async Task<IActionResult> Product(int productId)
        {
            var product = await _service.GetProductAsync(productId);

            var productVM = product.PrepareProductViewModel();

            return View(productVM);
        }


        [HttpPost]
        public async Task<ActionResult> AddPositionToCart(int productId, string userId)
        {
            if (userId == null) // jesli uzytkownik jest niezalogowany 
                return await AddAnonymousPositionToCart(productId);
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

        //[HttpPost] //partial 
        //public IActionResult Products(ProductsViewModel productsVM)
        //{

        //    var products = _service.Get(productsVM.FilterProducts.CategoryId,
        //    productsVM.FilterProducts.Name
        //    );

        //    return PartialView("_TasksTable", products);
        //}

        public async Task<IActionResult> Products()
        {
            var isCategoryListEmpty = await _service.IsProductCategoryListEmpty();
            if (isCategoryListEmpty)
            {
                await _service.CreateListOfCategories();
            }
            var products = await _service.GetProductsAsync();
            if (products.Count() == 0)
            {
                var emptyListOfProducts = new List<ProductViewModel>();
                return View(emptyListOfProducts);
            }

            var productsVM = products.ProductsIEnumerableToList();
            return View(productsVM);

        }

        [HttpPost] //partial 
        public IActionResult Products(ProductsViewModel productsVM)
        {

            var products = _service.Get(productsVM.FilterProducts.CategoryId,
            productsVM.FilterProducts.Name
            );

            return PartialView("_Products", products);
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
