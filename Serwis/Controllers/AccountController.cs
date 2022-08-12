using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models.Domains;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serwis.Converter;
using Serwis.Models.ViewModels;
using Serwis.Repository;

namespace Serwis.Controllers
{
    public class AccountController : Controller
    {

        private readonly AuthRepository _accountAuth;
        public const string SessionKeyName = "Login";
        private const string UserNotFound = "Nie znaleziono użytkownika!";
        private const string IsAdmin = "admin";
        private const string Cookie = "CookieAuth";
        private const string IsUser = "User";
        private const string Email = "Email";
        private const string Id = "Id";
        private const string Wallet = "Wallet";
        private const string Validation = "Walidacja";
        private const string True = "true";

        public AccountController(AuthRepository accountAuth)
        {
            _accountAuth = accountAuth;
        }
        public RegisterViewModel RegisterVM { get; set; }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register() //przekirowanie zrobic
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            RegisterVM = new RegisterViewModel(); // chyba najlepiej bedzie view model zrobic
            return View(RegisterVM);

        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel registerVM)
        {
            registerVM.CreatedDate = DateTime.Now;
            if (!ModelState.IsValid)
                return View(registerVM);

            if (registerVM.Password != registerVM.RepeatPassword) //po stronie uzytkownika również
            {
                TempData[Validation] = "Hasła nie sa takie same";
                return View(registerVM);
            }

            var usernameFromRegister = registerVM.UserName.ToLower();

            if (!IsUserNameFromRegisterViewModelValid(usernameFromRegister))
            {
                TempData[Validation] = "Uzytkownik o podanej nazwie juz istnieje";
                return View(registerVM);
            }

            try
            {
                var user = registerVM.ConvertToApplicationUserFromRegisterViewModel();

                _accountAuth.CreateUser(user);
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Login", "Account");

        }
        private bool IsUserNameFromRegisterViewModelValid(string username)
        {
            var isValid = _accountAuth.IsUserNameFromRegisterValid(username);
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
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                return View(userVM);
            }

            var user = userVM.ConvertToApplicationUser();

            var userFromLoginCredentials = UserFromLogin(user.UserName, user.Password);

            if (userFromLoginCredentials == null)
            {
                TempData["UserNotFound"] = UserNotFound;
                return View(userVM); //dac info ze nie ma takiego 
            }
            AddSessionForUserFrom(userFromLoginCredentials);

            var clasimsPrincipal = AddClaimsForUserFromLogin(user);

            await HttpContext.SignInAsync(Cookie, clasimsPrincipal);
            return RedirectToAction("Index", "Home");


        }
        private void AddSessionForUserFrom(ApplicationUser credential)
        {
            HttpContext.Session.SetString(SessionKeyName, credential.UserName); // zoabczyc co tu sie stanie w tej sytuacji nie wyswietla sie w narozniku
            HttpContext.Session.SetString(Email, credential.Email);// prawdopobonie to powinno byc w cookie
            HttpContext.Session.SetString(Id, (credential.Id).ToString()); // prawdopobonie to powinno byc w cookie
            HttpContext.Session.SetString(Wallet, (credential.Wallet).ToString()); // prawdopobonie to powinno byc w cookie
            //poniewaz po wylaczeniu przegladarki uzytkownik badac dalej zalogowanym 
            // nie bedzie widzial tych pozycji mozliwe problemy pozniej
            //ustawic dla uzytkownka mozliwosc nie wylogowywania
        }

        private ApplicationUser UserFromLogin(string userName, string password)
        {
            return _accountAuth.FindUserWithLoginCredentials(userName, password);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove(SessionKeyName); // zoabczyc co tu sie stanie w tej sytuacji nie wyswietla sie w narozniku
            HttpContext.Session.Remove(Email);// prawdopobonie to powinno byc w cookie
            HttpContext.Session.Remove(Id); // prawdopobonie to powinno byc w cookie
            HttpContext.Session.Remove(Wallet);
            await HttpContext.SignOutAsync(Cookie);
            return RedirectToAction("Index", "Home");
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
               // HttpContext.Session.SetString("Zwykly", userName);

                var identityUser = new ClaimsIdentity(claimsUser, Cookie);
                var clasimsPrincipal_user = new ClaimsPrincipal(identityUser);
                return clasimsPrincipal_user;
            }

        }

        [Authorize]
        public IActionResult UserInfo()
        {
            var sessionUser = HttpContext.Session.GetString(SessionKeyName);

            var user = _accountAuth.GetUser(sessionUser);

            var userVM = user.PrepareApplicationUserViewModel();
            return View(userVM);
        }
    }
}
