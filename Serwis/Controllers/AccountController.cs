using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serwis.Models.Domains;
using System.Security.Claims;

namespace Serwis.Controllers
{
    public class AccountController : Controller
    {
        [BindProperty]
        public Credential Credential { get; set; }


        public IActionResult Index()
        {
            if (!ModelState.IsValid)
            {
                Credential = new();
                return View(Credential);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Credential credential)
        {
            if (ModelState.IsValid)
            {
                if (Credential.UserName == "admin" && Credential.Password == "admin")
                {
                    var claims = AddClaims();
                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var clasimsPrincipal = new ClaimsPrincipal(identity);
                    ////kolejnosc ma znacznie dlatego w innych projektach nalezy trzymac sie tego ukladu 
                    //var authProperties = new AuthenticationProperties
                    //{
                    //    IsPersistent = Credential.RememberMe //opcja dziki ktorej cookie nie znika wraz z wylacznienim przegladarki
                    //};

                    await HttpContext.SignInAsync("MyCookieAuth", clasimsPrincipal);

                    return RedirectToAction("Service", "Home"); // przekierowanie do akcji w kontrolerze home
                };
            }
            return View(credential);
        }

        private List<Claim> AddClaims()
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
            return claims;
        }


        [Authorize(Policy = "AdminOnly")]
        public IActionResult Test()
        {

            return View();
        }
    }
}
