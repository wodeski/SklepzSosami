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

        public AccountController(AccountAuthRepository accountAuth)
        {
            _accountAuth = accountAuth;
        }
        [BindProperty]
        public Credential Credential { get; set; }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Credential credential)
        {
            if (!ModelState.IsValid)//dac js po stronie klienta
                return View(credential);

            if (!CheckAuth(credential))
            {
                TempData["UserNotFound"] = UserNotFound;
                return View(credential); //dac info ze nie ma takiego 
            }
            AddSession(credential);
            var clasimsPrincipal = AddClaims();

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
        private void AddSession(Credential credential)
        {
            HttpContext.Session.SetString(SessionKeyName, credential.UserName);
        }

        private bool CheckAuth(Credential credential)
        {
            return _accountAuth.FindUser(credential);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // najlepiej const string
            return RedirectToAction("Index", "Home");
        }

        private ClaimsPrincipal AddClaims()
        {
            var claims = new List<Claim>
                        {
                       // new Claim(ClaimTypes.Name, "admin"), //własciwosci jak mniemam
                       // new Claim(ClaimTypes.Email, "admin@wp.pl"), //własciwosci jak mniemam
                        new Claim("Department", "Orders")
                 // new Claim("Admin", "true"),
                 // new Claim("Manager", "true"),
                 // new Claim("EmploymentDate", "2022-04-01") //claim w ktorym bedzie informacje o dacie zatrudnienia
                 //CLAIM DODANIE GO DO COOKIE DAJW MOZLIWOSC WGLADU DO ZAWARTOSCI USTALONEJ W PROGRAM.CS
             };
            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var clasimsPrincipal = new ClaimsPrincipal(identity);
            return clasimsPrincipal;
        }
    }
}
