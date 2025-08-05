using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebSupport.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();
        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string emp_id, string password)
        {
            // Ví dụ kiểm tra username/password đơn giản
            var role = "User";
            if (emp_id.ToLower() == "admin") role = "Admin";
            if (password == "123")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, emp_id),
                    new Claim(ClaimTypes.Role, role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Login failed";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }
    }
}