using HPDQ.WebSupport.DataEntitites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
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
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                Email = email!,
                ProgressByEmail = email!,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.OR,
            };
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            return View(tickets);
        }

        [Authorize]
        public async Task<IActionResult> NewTicket()
        {
            var ticketTypes = await WebSupport.Utilities.API.Instance.CodeDetail.Load(new HPDQ.WebSupport.Criteria.CodeDetailCriteria
            {
                Master = CodeDetailMaster.TicketType,
            });
            return View(new NewTicketViewModel { TicketTypes = ticketTypes });
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
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                ProgressByEmail = null,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.ISNULL,
            };
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            ViewBag.Type = 1;
            return View("Index", tickets);
        }

        [Authorize]
        public async Task<IActionResult> History()
        {
            var email = _userEmail();
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                ProgressByEmail = null,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.ISNULL,
                ExcludeStatus = null,
            };
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            ViewBag.Type = 1;
            return View("Index", tickets);
        }
    }
}
