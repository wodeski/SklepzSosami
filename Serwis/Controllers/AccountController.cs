using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models.Domains;
using Serwis.Repository.AccountAuth;
using System.Security.Claims;

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
        [BindProperty]
        public ApplicationUser Credential { get; set; }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Register(ApplicationUser applicationUser)
        {
            try
            {
                Console.WriteLine("jest okej");
            }
            catch (Exception ex)
            {
                //logowanie do pliku
                return Json(new { Success = false, message = ex.Message });
            }
            return Json(new { Success = true });

        }
        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUser credential)
        {
            if (!ModelState.IsValid)//dac js po stronie klienta
                return View(credential);

            if (!CheckAuth(credential))
            {
                TempData["UserNotFound"] = UserNotFound;
                return View(credential); //dac info ze nie ma takiego 
            }
            AddSession(credential);
            var clasimsPrincipal = AddClaims(credential);

            //Action_();
            ////kolejnosc ma znacznie dlatego w innych projektach nalezy trzymac sie tego ukladu 

            await HttpContext.SignInAsync("MyCookieAuth", clasimsPrincipal);
            return RedirectToAction("Service", "Home");//, login); // przekierowanie do akcji w kontrolerze home


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
        }

        private bool CheckAuth(ApplicationUser credential)
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
                       // new Claim(ClaimTypes.Name, "admin"), //własciwosci jak mniemam
                       // new Claim(ClaimTypes.Email, "admin@wp.pl"), //własciwosci jak mniemam
                        new Claim("Admin", "true")
                 // new Claim("Admin", "true"),
                 // new Claim("Manager", "true"),
                 // new Claim("EmploymentDate", "2022-04-01") //claim w ktorym bedzie informacje o dacie zatrudnienia
                 //CLAIM DODANIE GO DO COOKIE DAJW MOZLIWOSC WGLADU DO ZAWARTOSCI USTALONEJ W PROGRAM.CS
                 
            };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var clasimsPrincipal = new ClaimsPrincipal(identity);
                return clasimsPrincipal;
            }

            var claims_user = new List<Claim>
            {
                new Claim("User", "true"),
            };
            var identity_user = new ClaimsIdentity(claims_user, "MyCookieAuth");
            var clasimsPrincipal_user = new ClaimsPrincipal(identity_user);
            return clasimsPrincipal_user;

        }
    }
}
