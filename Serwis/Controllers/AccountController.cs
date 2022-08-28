using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models.Domains;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serwis.Converter;
using Serwis.Models.ViewModels;
using Serwis.Repository;
using Serwis.Core.Service;
using Serwis.Converters;

namespace Serwis.Controllers
{
    public class AccountController : Controller
    {

        private readonly IService _service;
        public const string SessionKeyName = "Login";
        private const string UserNotFound = "Nie znaleziono użytkownika!";
        public const string IsAdmin = "admin";
        public const string IsAnonymous = "anonim";
        public const string Cookie = "CookieAuth";
        private const string IsUser = "User";
        private const string Email = "Email";
        public const string Id = "Id";
        public const string Password = "Password";
        private const string Validation = "Walidacja";
        private const string True = "true";
        public const string UserOnly = "UserOnly";
        public AccountController(IService service)
        {
            _service = service;
        }
        public RegisterViewModel RegisterVM { get; set; }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Shop");

            RegisterVM = new RegisterViewModel();
            return View(RegisterVM);

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            registerVM.CreatedDate = DateTime.Now;
            if (!ModelState.IsValid)
                return View(registerVM);
            if(registerVM.UserName != IsAnonymous)
            {
                TempData[Validation] = "Logowanie niemożliwe";
                return View(registerVM);
            }

            if (registerVM.Password != registerVM.RepeatPassword)
            {
                TempData[Validation] = "Hasła nie sa takie same";
                return View(registerVM);
            }

            var usernameFromRegister = registerVM.UserName.ToLower();
            var isUserValid = await IsUserNameFromRegisterViewModelValid(usernameFromRegister);
            if (!isUserValid)
            {
                TempData[Validation] = "Uzytkownik o podanej nazwie juz istnieje";
                return View(registerVM);
            }

            try
            {
                var user = registerVM.ConvertToApplicationUserFromRegisterViewModel();

                await _service.CreateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Login", "Account");

        }
        private async Task<bool> IsUserNameFromRegisterViewModelValid(string username)
        {
            var isValid = await _service.IsUserNameFromRegisterValid(username);
            if (isValid)
            {
                return true;
            }
            return false;
        }
        [HttpPost]

        public async Task<IActionResult> Login(ApplicationUserViewModel userVM)
        {
            if (!ModelState.IsValid)
            {
                //var errors = ModelState
                //    .Where(x => x.Value.Errors.Count > 0)
                //    .Select(x => new { x.Key, x.Value.Errors })
                //    .ToArray();
                return View(userVM);
            }

            var user = userVM.ConvertToApplicationUser();

            var userFromLogin = await UserFromLogin(user.UserName, user.Password);

            if (userFromLogin == null)
            {
                TempData["UserNotFound"] = UserNotFound;
                return View(userVM);
            }
            AddSessionForUserFrom(userFromLogin);

            var clasimsPrincipal = AddClaimsForUserFromLogin(user);

            //var authentication = new AuthenticationProperties
            //{
            //    IsPersistent = userVM.RememberMe,
            //};

            await HttpContext.SignInAsync(Cookie, clasimsPrincipal); //, authentication);
            return RedirectToAction("Index", "Shop");
        }
        public void AddSessionForUserFrom(ApplicationUser credential)
        {
            //nie działa jak trzeb, w momencie wyłaczenia przegladarki sesja wygasa 
            HttpContext.Session.SetString(SessionKeyName, credential.UserName); 
            HttpContext.Session.SetString(Email, credential.Email);
            HttpContext.Session.SetString(Id, (credential.Id).ToString()); 
            HttpContext.Session.SetString(Password, credential.Password);
        }
        private async Task<ApplicationUser> UserFromLogin(string userName, string password)
        {
            return await _service.FindUserWithLoginCredentials(userName, password);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove(SessionKeyName); 
            HttpContext.Session.Remove(Email);
            HttpContext.Session.Remove(Id); 
            HttpContext.Session.Remove(Password);
            await HttpContext.SignOutAsync(Cookie);
            return RedirectToAction("Index", "Shop");
        }
        private ClaimsPrincipal AddClaimsForUserFromLogin(ApplicationUser user)
        {
            var userName = user.UserName.ToLower();
            if (userName == IsAdmin)
            {
                var claims = new List<Claim>
                {
                    new Claim(IsAdmin, True)
                };
                var identity = new ClaimsIdentity(claims, Cookie);
                var clasimsPrincipal = new ClaimsPrincipal(identity);
                return clasimsPrincipal;
            }
            else
            {
                var claimsUser = new List<Claim>
                {
                    new Claim(IsUser, True),
                };
                var identityUser = new ClaimsIdentity(claimsUser, Cookie);
                var clasimsPrincipal_user = new ClaimsPrincipal(identityUser);
                return clasimsPrincipal_user;
            }

        }

        [Authorize(Policy = UserOnly)]
        public async Task<IActionResult> UserInfo()
        {
            var sessionUserId = HttpContext.Session.GetString(Id);
            
            //sprawdizic czy jest mozlwiosc pustej sesji
            var orders = await _service.GetOrdersForUserAsync(new Guid(sessionUserId));
            var ordersVM = orders.OrderIEnumerableToList();
            return View(ordersVM);
        }
    }
}
