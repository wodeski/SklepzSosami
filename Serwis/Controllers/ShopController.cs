using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Controllers;
using Serwis.Converter;
using Serwis.Converters;
using Serwis.Core.Models;
using Serwis.Core.Service;
using Serwis.Core.ViewModels;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.Extensions;
using Serwis.Models.ViewModels;
using Serwis.Persistance;
using Serwis.Repository;
using System.Text.Json;

namespace Serwis.ShopControllers
{

    public class ShopController : Controller // dodaje wsparcie dla widoków
    {
        private readonly IService _service;
        private List<OrderPositionViewModel> _orderPositions;
        public static string AnonymousId = "";
        public string Anonymous = "anonim";
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
                var isCategoryListEmpty = await _service.IsProductCategoryListEmpty();
                if (isCategoryListEmpty)
                {
                    await _service.CreateListOfCategories();
                }
                var categories = await _service.GetListOfProductCategories();

                var adminCreated = await _service.IsAdminCreated();
                var anonymousCreated = await _service.IsAnonymousCreated();

                var dbHasproducts = await _service.DbHasAnyProducts();

                if (dbHasproducts == false)
                    await _service.InsertProducts();

                if (adminCreated == false)
                    await _service.CreateAdmin();
                
                if (anonymousCreated == false)
                    await _service.CreateAnonymous();

                var anonymous = await _service.FindUserAsync(Anonymous);
                AnonymousId = anonymous.Id.ToString();
                var products = await _service.GetProductsAsync();

                productsViewModel = new ProductsViewModel
                {
                    FilterProducts = new FilterProducts(),
                    Products = products,
                    Categories = categories
                };
            }
            catch (Exception ex)
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

        public async Task<IActionResult> Cart(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                //anonimowy uzytkownik
                var data = HttpContext.Session.GetComplexData<List<OrderPositionViewModel>>(AnonymousCart);
                return View(data);
            }
            var orderPositions = await _service.GetOrderPositionsForUserAsync(userId);//usuwa wszystkue te ktore mg byc false i byc ich wiecej kak jedno

            var orderPositionsVm = orderPositions.OrderPositionsIEnumerableToList(); // tu jest błąd??? jaki.>??
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
            var order = orderVM.ConvertToOrder(); // czy view model jest bez id uzytkownika?

            // poki co wysylanie maila dla anonima
            await _service.CreateOrderAsync(order); // zapisanie zamówienia do bazy zmienic na zapisywanie zamowienia gotowego

            try
            {
                foreach (var position in orderVM.OrderPositions)
                {
                    try
                    {
                        await _service.UpdateOrderPositionAsync(position, order);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }


                _service.SendMail(email, orderVM);

                if (HttpContext.Session.GetString(Id) == null)
                {
                    var data = HttpContext.Session.GetComplexData<List<OrderPositionViewModel>>(AnonymousCart);
                    foreach (var position in data)
                    {
                        await RemoveAnonymousPositionFromCart(position.ProductId);
                    }
                }
                else
                {
                    var orderPositions = await _service.GetOrderPositionsForUserAsync(new Guid(HttpContext.Session.GetString(Id)));//usuwa wszystkue te ktore mg byc false i byc ich wiecej kak jedno

                    var orderPositionsVm = orderPositions.OrderPositionsIEnumerableToList();

                    foreach (var position in orderPositionsVm)
                    {
                        await DeleteUserOrderPositionFromCart(position.ProductId.ToString(), position.OrderId);
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true }); // zrobic przekierwoanie do strony głownej

        }


        public async Task<IActionResult> DeleteUserOrderPositionFromCart(string productId, int orderId)
        {
            var sessionUserId = HttpContext.Session.GetString(Id);

            if (sessionUserId == null)
            {
                await RemoveAnonymousPositionFromCart(Convert.ToInt32(productId));
                return Json(new { Success = true });
            }
            var userId = new Guid(sessionUserId);

            await _service.DeleteOrderPositionAsync(orderId, userId, Convert.ToInt32(productId));

            var listOfOrderPositions = await _service.CountOrderPositions(orderId, userId);

            if (listOfOrderPositions == 0) // usun zamowienie jesli nie ma w nim zadnych pozycji
            {
                await _service.DeleteOrder(userId, orderId);

            }

            return Json(new { Success = true });

        }
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

        public async Task<IActionResult> Product(int productId)
        {
            var product = await _service.GetProductAsync(productId);

            var productVM = product.PrepareProductViewModel();

            return View(productVM);
        }


        [HttpPost]
        public async Task<ActionResult> AddPositionToCart(int productId, Guid userId)
        {
            if (userId == Guid.Empty) // jesli uzytkownik jest niezalogowany 
                return await AddAnonymousPositionToCart(productId);
            try
            {
                await _service.AddOrderPositionAsync(userId, productId);
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });
        }

        public async Task<IActionResult> Products()
        {
            
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

            var products = _service.Get(productsVM.FilterProducts.CategoryId, productsVM.FilterProducts.Name);
            //tak nie powinno byc, do zmiany
            var productListVM = products.ProductsIEnumerableToList();
            //proba przekazani anowego viewmodelu do index

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
                Quantity = 1,
                ProductId = productId
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
