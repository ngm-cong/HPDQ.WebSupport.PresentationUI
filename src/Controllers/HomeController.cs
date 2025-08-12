using HPDQ.WebSupport.DataEntitites;
using HPDQ.WebSupport.Models;
using HPDQ.WebSupport.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace HPDQ.WebSupport.Controllers
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
            var tickets = await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            return View(tickets);
        }

        /// <summary>
        /// Hiển thị trang để tạo một yêu cầu mới.
        /// </summary>
        /// <returns>View với một ViewModel chứa danh sách các loại vấn đề của yêu cầu.</returns>
        [Authorize]
        public async Task<IActionResult> NewTicket()
        {
            var ticketTypes = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new HPDQ.WebSupport.Criteria.CodeDetailCriteria
            {
                Master = CodeDetailMaster.TicketType,
            });
            return View(new NewTicketViewModel { TicketTypes = ticketTypes });
        }

        /// <summary>
        /// Hiển thị trang giao diện để đăng nhập hệ thống.
        /// </summary>
        /// <returns>Giao diện đăng nhập hệ thống.</returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Hiển thị trang lỗi chung của ứng dụng.
        /// </summary>
        /// <remarks>
        /// Action này được cấu hình để ngăn việc lưu trữ cache của phản hồi, đảm bảo
        /// người dùng luôn nhận được thông báo lỗi mới nhất.
        /// </remarks>
        /// <returns>
        /// Một View hiển thị thông tin lỗi, bao gồm ID của yêu cầu (Request ID).
        /// </returns>
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
            var tickets = await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
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
            var tickets = await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            return View("Index", tickets);
        }

        /// <summary>
        /// Hiển thị Dashboard.
        /// </summary>
        /// <returns>View Dashboard.</returns>
        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }

        /// <summary>Lớp cấu trúc dữ liệu của sơ đồ.</summary>
        private class LineChartModel
        {
            /// <summary>Tiêu đề hiển thị của một đối tượng dữ liệu trên sơ đồ.</summary>
            public string Label { get; set; } = string.Empty;

            /// <summary>Danh sách các giá trị của một đối tượng (Thứ tự tương ứng với danh sách tiêu đề của sơ đồ).</summary>
            public IEnumerable<int>? Data { get; set; }

            /// <summary>Màu khung viền của một đối tượng dữ liệu sẽ hiển thị trên sơ đồ.</summary>
            public IEnumerable<string>? BorderColor { get; set; }

            /// <summary>Màu khung nền của một đối tượng dữ liệu sẽ hiển thị trên sơ đồ.</summary>
            public IEnumerable<string>? BackgroundColor { get; set; }

            /// <summary>Tỉ lệ bo cong của những đường dữ liệu (Nhận giá trị từ 0-1).</summary>
            public double Tension { get; set; }
        }

        /// <summary>
        /// Lới cấu trúc dữ liệu dành cho hiển thị dashboard số lượng yêu cầu.
        /// </summary>
        private class DashboardViewModel
        {
            public IEnumerable<string>? Labels { get; set; }
            public IEnumerable<LineChartModel>? Datasets { get; set; }
        }

        /// <summary>Dữ liệu hiển thị lên dashboard số lượng yêu cầu.</summary>
        /// <returns>Đối tượng mới của <see cref="DashboardViewModel"/>.</returns>
        [Authorize]
        public async Task<APIResult> DashboardViewData(int? rptType)
        {
            var criteria = new Criteria.TicketCriteria
            {
                CreatedOnFr = DateTime.Now.Date.AddDays(-1 * DateTime.Now.Day),
                ExcludeStatus = null,
            };
            var ticketsInMonth = await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
            switch (rptType)
            {
                case 0:
                    var issueTypes = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new Criteria.CodeDetailCriteria { Master = CodeDetailMaster.TicketType }) ?? new List<CodeDetail>();
                    var rptByIssueValues = from x in ticketsInMonth
                                           join t in issueTypes on x.Type equals t.Code
                                           group x by t.Description into g
                                           orderby g.Key
                                           select new
                                           {
                                               Label = g.Key,
                                               Total = g.Count(),
                                           };
                    var labels = issueTypes.Select(x => x.Description).OrderBy(x => x).ToList();
                    var outVal = new APIResult<DashboardViewModel>
                    {
                        Data = new DashboardViewModel
                        {
                            Labels = labels,
                            Datasets = new List<LineChartModel>
                            {
                                new LineChartModel
                                {
                                    Label = "Số lượng yêu cầu",
                                    Data = from l in labels
                                           join d in rptByIssueValues on l equals d.Label into ld
                                           from d in ld.DefaultIfEmpty()
                                           select d?.Total ?? 0,
                                    BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Take(labels.Count()),
                                    BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Take(labels.Count()),
                                    Tension = 0.4,
                                }
                            }
                        }
                    };
                    return outVal;
                case 1:
                    var rptValues = from x in ticketsInMonth
                                    group x by x.CreatedOn.Day into g
                                    select new
                                    {
                                        Date = g.Key,
                                        Total = g.Count(),
                                        Processed = g.Count(x => x.Status == TicketStatus.Done || x.Status == TicketStatus.Closed),
                                        InProgress = g.Count(x => x.Status == TicketStatus.InProgress),
                                    };
                    labels = new List<string> { "Yêu cầu mới", "Đang xử lý", "Đã xử lý" };
                    var inProgress = rptValues.Sum(x => x.InProgress);
                    var processed = rptValues.Sum(x => x.Processed);
                    return new APIResult<DashboardViewModel>
                    {
                        Data = new DashboardViewModel
                        {
                            Labels = labels,
                            Datasets = new List<LineChartModel>() {
                                new LineChartModel
                                {
                                    Label = "Số lượng yêu cầu",
                                    Data = new List<int> { rptValues.Sum(x => x.Total) - inProgress - processed, inProgress, processed },
                                    BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Take(labels.Count()),
                                    BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Take(labels.Count()),
                                    Tension = 0.4,
                                },
                            },
                        }
                    };
                default:
                    return null;
            }
        }
    }
}