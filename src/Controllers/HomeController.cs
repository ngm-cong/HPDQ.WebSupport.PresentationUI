using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using WebSupport.Models;

namespace WebSupport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private string? _userEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("preferred_username")?.Value
                ?? User.FindFirst("upn")?.Value;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var email = _userEmail();
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(email!);
            return View(tickets);
        }

        [Authorize]
        public IActionResult NewTicket()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var email = _userEmail();
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.LoadByProgress(email!);
            return View("Index", tickets);
        }
    }
}
