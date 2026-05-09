using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Oswald_POS.Data;
using System.Security.Claims;

namespace Oswald_POS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var worker = _context.Workers
                .FirstOrDefault(w => w.Username == username && w.Password == password);

            if (worker == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, worker.FullName),
                new Claim(ClaimTypes.Role, worker.Role),
                new Claim("WorkerId", worker.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}