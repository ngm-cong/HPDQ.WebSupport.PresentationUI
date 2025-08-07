using HPDQ.WebSupport.DataEntitites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebSupport.Models;

namespace WebSupport.Controllers
{
    /// <summary>
    /// Controller chính để xử lý các yêu cầu liên quan đến yêu cầu hỗ trợ.
    /// </summary>
    /// <remarks>
    /// Controller này cung cấp các hành động (action) để hiển thị danh sách yêu cầu của người dùng,
    /// tạo yêu cầu mới, xem danh sách yêu cầu cần xử lý (cho admin), và xem lịch sử yêu cầu.
    /// </remarks>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Khởi tạo một đối tượng mới của <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">Logger để ghi lại thông tin.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lấy mã nhân viên của người dùng hiện tại từ Claims.
        /// </summary>
        /// <returns>Mã nhân viên dưới dạng chuỗi hoặc null nếu không tìm thấy.</returns>
        private string? _userEMP_ID()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("preferred_username")?.Value
                ?? User.FindFirst("upn")?.Value;
        }

        /// <summary>
        /// Hiển thị danh sách yêu cầu của người dùng hiện tại, bao gồm các yêu cầu đã tạo và các yêu cầu đang xử lý.
        /// </summary>
        /// <returns>View chứa danh sách các yêu cầu.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var emp_id = _userEMP_ID();
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                EMP_ID = emp_id!,
                ProgressBy_EMP_ID = emp_id!,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.OR,
            };
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            return View(tickets);
        }

        /// <summary>
        /// Hiển thị trang để tạo một yêu cầu mới.
        /// </summary>
        /// <returns>View với một ViewModel chứa danh sách các loại vấn đề của yêu cầu.</returns>
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

        /// <summary>
        /// Hiển thị danh sách các yêu cầu chưa được gán cho nhân viên nào.
        /// </summary>
        /// <remarks>
        /// Action này yêu cầu quyền "Admin" để truy cập.
        /// </remarks>
        /// <returns>View chứa danh sách các yêu cầu chưa được xử lý.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var emp_id = _userEMP_ID();
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                ProgressBy_EMP_ID = null,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.ISNULL,
            };
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            ViewBag.Type = 1;
            return View("Index", tickets);
        }

        /// <summary>
        /// Hiển thị lịch sử các yêu cầu của người dùng, bao gồm cả các yêu cầu đã đóng.
        /// </summary>
        /// <returns>View chứa lịch sử danh sách các yêu cầu.</returns>
        [Authorize]
        public async Task<IActionResult> History()
        {
            var emp_id = _userEMP_ID();
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                ExcludeStatus = null,
                EMP_ID = emp_id,
                ProgressBy_EMP_ID = emp_id,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.OR,
            };
            var tickets = await WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            ViewBag.Type = 1;
            return View("Index", tickets);
        }
    }
}
