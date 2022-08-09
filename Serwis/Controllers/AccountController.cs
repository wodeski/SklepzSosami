using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models.Domains;
using Serwis.Repository.AccountAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serwis.Converter;

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
        public Register Credential { get; set; }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register() //przekirowanie zrobic
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home"); 

            Credential = new Register(); // chyba najlepiej bedzie view model zrobic
            return View();

        }

        [HttpPost]
        public IActionResult Register(Register register) //trzeba doprowadzic do sytuacji w kótrej sprawdzone zostanie register tylko
        {
            register.CreatedDate = DateTime.Now;    
            if (!ModelState.IsValid)
                return View(register);
            if (register.Password != register.RepeatPassword)
            {
                TempData["Walidacja"] = "Hasła nie sa takie same";
                return View(register);
            }
            try
            {
                var user = register.ConvertToApplicationUser();
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
        public async Task<IActionResult> Login(ApplicationUser credential)
        {
            if (!ModelState.IsValid)
            {
                
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                return View(credential);
            }
            var userInDatabase = CheckAuth(credential);
            if (userInDatabase == null)
            {
                TempData["UserNotFound"] = UserNotFound;
                return View(credential); //dac info ze nie ma takiego 
            }
            AddSession(userInDatabase);
            var clasimsPrincipal = AddClaims(credential);

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

        private ClaimsPrincipal AddClaims(ApplicationUser credential)
        {

            if (credential.UserName.ToLower() == AdminIsSigningIn)
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
                HttpContext.Session.SetString("Zwykly", credential.UserName);


                var identity_user = new ClaimsIdentity(claims_user, "MyCookieAuth");
                var clasimsPrincipal_user = new ClaimsPrincipal(identity_user);
                return clasimsPrincipal_user;
            }

        }

        [Authorize]
        public IActionResult UserInfo()
        {
            var credential = HttpContext.Session.GetString(SessionKeyName);

            var user = _accountAuth.GetUser(credential);
            return View(user);
        }
    }
}
