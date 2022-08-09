using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models.Domains;
using Serwis.Repository.AccountAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serwis.Converter;
using Serwis.Models.ViewModels;

namespace Serwis.Controllers
{
    public class AccountController : Controller
    {

        private readonly AccountAuthRepository _accountAuth;
        public const string SessionKeyName = "Login";
        private const string UserNotFound = "Nie znaleziono użytkownika!";
        private const string AdminIsSigningIn = "admin";

        public AccountController(AccountAuthRepository accountAuth)
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
        public IActionResult Register(RegisterViewModel registerVM) //trzeba doprowadzic do sytuacji w kótrej sprawdzone zostanie register tylko
        {
            registerVM.CreatedDate = DateTime.Now;  // sprawdzic czy dziala w instrukcji  
            if (!ModelState.IsValid)
                return View(registerVM);
            if (registerVM.Password != registerVM.RepeatPassword)
            {
                TempData["Walidacja"] = "Hasła nie sa takie same";
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

            var userInDatabase = CheckAuth(user);
            if (userInDatabase == null)
            {
                TempData["UserNotFound"] = UserNotFound;
                return View(userVM); //dac info ze nie ma takiego 
            }
            AddSession(userInDatabase);
            var clasimsPrincipal = AddClaims(user);

            //Action_();
            ////kolejnosc ma znacznie dlatego w innych projektach nalezy trzymac sie tego ukladu 

            await HttpContext.SignInAsync("MyCookieAuth", clasimsPrincipal);
            return RedirectToAction("Index", "Home");//, login); // przekierowanie do akcji w kontrolerze home


        }

        private void Action_()
        {
            //var authProperties = new AuthenticationProperties
            //{
            //    IsPersistent = Credential.RememberMe //opcja dziki ktorej cookie nie znika wraz z wylacznienim przegladarki
            //};
        }
        private void AddSession(ApplicationUser credential)
        {
            HttpContext.Session.SetString(SessionKeyName, credential.UserName);
            HttpContext.Session.SetString("Email", credential.Email);
            HttpContext.Session.SetString("Id", (credential.Id).ToString());
            HttpContext.Session.SetString("Wallet", (credential.Wallet).ToString());

        }

        private ApplicationUser CheckAuth(ApplicationUser credential)
        {
            return _accountAuth.FindUser(credential);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // najlepiej const string
            return RedirectToAction("Index", "Home");
        }

        private ClaimsPrincipal AddClaims(ApplicationUser user)
        {

            if (user.UserName.ToLower() == AdminIsSigningIn)
            {
                var claims = new List<Claim>
                {
                    new Claim("Admin", "true")
                // new Claim(ClaimTypes.Name, "admin"), //własciwosci jak mniemam
                // new Claim(ClaimTypes.Email, "admin@wp.pl"), //własciwosci jak mniemam
                // new Claim("Admin", "true"),
                // new Claim("Manager", "true"),
                // new Claim("EmploymentDate", "2022-04-01") //claim w ktorym bedzie informacje o dacie zatrudnienia
                //CLAIM DODANIE GO DO COOKIE DAJW MOZLIWOSC WGLADU DO ZAWARTOSCI USTALONEJ W PROGRAM.CS
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var clasimsPrincipal = new ClaimsPrincipal(identity);
                return clasimsPrincipal;
            }
            else
            {

                var claims_user = new List<Claim>
                {
                    new Claim("User", "true"),
                };
                HttpContext.Session.SetString("Zwykly", user.UserName);


                var identity_user = new ClaimsIdentity(claims_user, "MyCookieAuth");
                var clasimsPrincipal_user = new ClaimsPrincipal(identity_user);
                return clasimsPrincipal_user;
            }

        }

        [Authorize]
        public IActionResult UserInfo()
        {
            var sessionUser = HttpContext.Session.GetString(SessionKeyName);

            var user = _accountAuth.GetUser(sessionUser);

            var userVM = PrepareApplicationUserViewModel(user);
            return View(userVM);
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
                Orders = user.Orders


            };

            return userVM;
        }
    }
}
