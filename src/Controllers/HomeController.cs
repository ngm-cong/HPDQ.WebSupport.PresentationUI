using HPDQ.WebSupport.DataEntitites;
using HPDQ.WebSupport.Models;
using HPDQ.WebSupport.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
        /// Sao chép các thuộc tính (properties) có cùng tên và kiểu dữ liệu từ một đối tượng sang một đối tượng mới.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng đầu ra.</typeparam>
        /// <param name="input">Đối tượng đầu vào chứa dữ liệu cần sao chép.</param>
        /// <returns>Một đối tượng mới có kiểu T đã được sao chép dữ liệu.</returns>
        private T CopyValue<T>(object input)
        {
            T output = Activator.CreateInstance<T>();
            var properties = input.GetType().GetProperties();
            var outProperties = typeof(T).GetProperties();
            properties = (from p in properties
                          join op in outProperties on new { p.Name, p.PropertyType } equals new { op.Name, op.PropertyType }
                          select p).ToArray();
            foreach (var prop in properties)
            {
                prop.SetValue(output, prop.GetValue(input, null));
            }
            return output;
        }

        /// <summary>Hàm lấy các dữ liệu dùng chung cho view Home.Index.</summary>
        /// <param name="tickets">Danh sách yêu cầu hiển thị lên view Home.Index.</param>
        /// <returns>Danh sách dữ liệu loại yêu cầu hiển thị bổ sung lên view Home.Index.</returns>
        private async Task<IEnumerable<CodeDetail>> LoadAndFillTicketTypes(List<HomeViewModel>? tickets)
        {
            var ticketTypes = (await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new Criteria.CodeDetailCriteria { Master = CodeDetailMaster.TicketType }))!;
            if (tickets?.Count > 0)
            {
                (from t in tickets
                 join tt in ticketTypes on t.Type equals tt.Code
                 select new { t, tt }).ToList().ForEach(x => x.t.TypeDescription = x.tt.Description);
            }
            return ticketTypes;
        }

        /// <summary>
        /// Hiển thị danh sách yêu cầu của người dùng hiện tại, bao gồm các yêu cầu đã tạo và các yêu cầu đang xử lý.
        /// </summary>
        /// <returns>View chứa danh sách các yêu cầu.</returns>
        [Authorize]
        public async Task<IActionResult> Index(byte? ticketType = null, DataEntitites.TicketStatus? ticketStatus = null
            , string? searchText = null)
        {
            try
            {
                var emp_id = _userEMP_ID();
                var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
                {
                    EMP_ID = emp_id!,
                    ProgressBy_EMP_ID = emp_id!,
                    SearchOption = HPDQ.WebSupport.Criteria.SearchOption.OR,
                };
                if (ticketType != null) criteria.Type = ticketType;
                if (ticketStatus != null) criteria.Status = ticketStatus;
                if (string.IsNullOrEmpty(searchText) == false) criteria.SearchText = searchText;
                var tickets = (await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria))?.Select(x => CopyValue<HomeViewModel>(x)).ToList();
                ViewBag.TicketTypes = await LoadAndFillTicketTypes(tickets);
                return View(tickets);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Hiển thị trang để tạo một yêu cầu mới.
        /// </summary>
        /// <returns>View với một ViewModel chứa danh sách các loại vấn đề của yêu cầu.</returns>
        [Authorize]
        [Route("taomoiyeucau")]
        public async Task<IActionResult> NewTicket()
        {
            var ticketTypes = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new HPDQ.WebSupport.Criteria.CodeDetailCriteria
            {
                Master = CodeDetailMaster.TicketType,
            });
            return View(new NewTicketViewModel { TicketTypes = ticketTypes });
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
        [Route("theodoiyeucau")]
        public async Task<IActionResult> List(byte? ticketType = null, DataEntitites.TicketStatus? ticketStatus = null
            , string? searchText = null)
        {
            var emp_id = _userEMP_ID();
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                ProgressBy_EMP_ID = null,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.ISNULL,
            };
            if (ticketType != null) criteria.Type = ticketType;
            if (ticketStatus != null) criteria.Status = ticketStatus;
            if (string.IsNullOrEmpty(searchText) == false) criteria.SearchText = searchText;
            var tickets = (await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria))?.Select(x => CopyValue<HomeViewModel>(x)).ToList();
            ViewBag.TicketTypes = await LoadAndFillTicketTypes(tickets);
            ViewBag.Type = 1;
            ViewData["Title"] = "Theo dõi yêu cầu";
            return View("Index", tickets);
        }

        /// <summary>
        /// Hiển thị lịch sử các yêu cầu của người dùng, bao gồm cả các yêu cầu đã đóng.
        /// </summary>
        /// <returns>View chứa lịch sử danh sách các yêu cầu.</returns>
        [Authorize]
        [Route("lichsuyeucau")]
        public async Task<IActionResult> History(byte? ticketType = null, DataEntitites.TicketStatus? ticketStatus = null
            , string? searchText = null)
        {
            var emp_id = _userEMP_ID();
            var criteria = new HPDQ.WebSupport.Criteria.TicketCriteria
            {
                ExcludeStatus = null,
                EMP_ID = emp_id,
                ProgressBy_EMP_ID = emp_id,
                SearchOption = HPDQ.WebSupport.Criteria.SearchOption.OR,
            };
            if (ticketType != null) criteria.Type = ticketType;
            if (ticketStatus != null) criteria.Status = ticketStatus;
            if (string.IsNullOrEmpty(searchText) == false) criteria.SearchText = searchText;
            var tickets = (await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria))?.Select(x => CopyValue<HomeViewModel>(x)).ToList();
            ViewBag.TicketTypes = await LoadAndFillTicketTypes(tickets);
            ViewData["Title"] = "Lịch sử yêu cầu";
            return View("Index", tickets);
        }

        /// <summary>
        /// Hiển thị Dashboard.
        /// </summary>
        /// <returns>View Dashboard.</returns>
        [Authorize]
        [Route("dashboard")]
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
            public IEnumerable<double>? Data { get; set; }

            /// <summary>Màu khung viền của một đối tượng dữ liệu sẽ hiển thị trên sơ đồ.</summary>
            public IEnumerable<string>? BorderColor { get; set; }

            /// <summary>Màu khung nền của một đối tượng dữ liệu sẽ hiển thị trên sơ đồ.</summary>
            public IEnumerable<string>? BackgroundColor { get; set; }

            /// <summary>Tỉ lệ bo cong của những đường dữ liệu (Nhận giá trị từ 0-1).</summary>
            public double Tension { get; set; }

            /// <summary>Độ dày của các line trên biểu đồ.</summary>
            public double BorderWidth { get; set; }
        }

        /// <summary>
        /// Lới cấu trúc dữ liệu dành cho hiển thị dashboard số lượng yêu cầu.
        /// </summary>
        private class DashboardViewModel
        {
            public IEnumerable<string>? Labels { get; set; }
            public IEnumerable<LineChartModel>? Datasets { get; set; }
        }

        /// <summary>
        /// Hàm tạo dữ liệu biểu bồ số lượng yêu cầu nhóm bởi phân loại theo tháng.
        /// </summary>
        /// <param name="ticketsInMonth">Danh sách yêu cầu cần tạo dữ liệu biểu đồ.</param>
        /// <param name="ticketTypes">Danh sách các loại yêu cầu cần tạo dữ liệu biểu đồ.</param>
        /// <returns>Đối tượng hiển thị dữ liệu biểu bồ số lượng yêu cầu nhóm bởi phân loại theo tháng</returns>
        private DashboardViewModel RptType0(IEnumerable<Ticket>? ticketsInMonth
            , IEnumerable<CodeDetail> ticketTypes)
        {
            var rptByIssueValues = from x in ticketsInMonth
                                   join t in ticketTypes on x.Type equals t.Code
                                   group x by t.Description into g
                                   orderby g.Key
                                   select new
                                   {
                                       Label = g.Key,
                                       Total = g.Count(),
                                   };
            var labels = ticketTypes.Select(x => x.Description).OrderBy(x => x).ToList();
            var outVal = new DashboardViewModel
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
                                           select d?.Total ?? 0d,
                                    BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Take(labels.Count()),
                                    BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Take(labels.Count()),
                                    Tension = 0.4,
                                }
                            }
            };
            return outVal;
        }

        /// <summary>
        /// Hàm tạo dữ liệu biểu đồ nhóm tình trạng yêu cầu đã được tạo trong tháng.
        /// </summary>
        /// <param name="ticketsInMonth">Danh sách yêu cầu cần tạo dữ liệu biểu đồ.</param>
        /// <returns>Đối tượng hiển thị biểu đồ nhóm tình trạng yêu cầu đã được tạo trong tháng.</returns>
        private DashboardViewModel RptType1(IEnumerable<Ticket>? ticketsInMonth)
        {
            var rptValues = from x in ticketsInMonth
                            group x by x.CreatedOn.Day into g
                            select new
                            {
                                Date = g.Key,
                                Total = g.Count(),
                                Processed = g.Count(x => x.Status == TicketStatus.Done || x.Status == TicketStatus.Closed),
                                InProgress = g.Count(x => x.Status == TicketStatus.InProgress),
                            };
            var labels = new List<string> { "Yêu cầu mới", "Đang xử lý", "Đã xử lý" };
            var inProgress = rptValues.Sum(x => x.InProgress);
            var processed = rptValues.Sum(x => x.Processed);
            return new DashboardViewModel
            {
                Labels = labels,
                Datasets = new List<LineChartModel>() {
                    new LineChartModel
                    {
                        Label = "Số lượng yêu cầu",
                        Data = new List<double> { rptValues.Sum(x => x.Total) - inProgress - processed, inProgress, processed },
                        BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Take(labels.Count()),
                        BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Take(labels.Count()),
                        Tension = 0.4,
                    },
                },
            };
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
                    var ticketTypes = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new Criteria.CodeDetailCriteria { Master = CodeDetailMaster.TicketType }) ?? new List<CodeDetail>();
                    var outVal = new APIResult<DashboardViewModel>
                    {
                        Data = RptType0(ticketsInMonth, ticketTypes),
                    };
                    return outVal;
                case 1:
                    return new APIResult<DashboardViewModel>
                    {
                        Data = RptType1(ticketsInMonth),
                    };
                case 2:
                    var processedTickets = ticketsInMonth?.Where(x => x.Status == TicketStatus.Done || x.Status == TicketStatus.Closed) ?? new List<Ticket>();
                    var ticketStatusHistoryCriteria = new Criteria.TicketStatusHistoryCriteria
                    {
                        TicketIds = processedTickets.Select(x => x.Id),
                    };
                    var ticketStatusHistories = await HPDQ.WebSupport.Utilities.API.Instance.TicketStatusHistory.Load(ticketStatusHistoryCriteria) ?? new List<TicketStatusHistory>();
                    var historyDatas = ticketStatusHistories.GroupBy(x => x.TicketId).Select(x => new { TicketId = x.Key, ProcessedOn = x.Max(c => c.CreatedOn) });
                    var rptByTimeValues = from x in ticketsInMonth
                                          join h in historyDatas on x.Id equals h.TicketId
                                          group new { x.CreatedOn, h.ProcessedOn } by x.CreatedOn.Day / 7 + 1 into g
                                          select new
                                          {
                                              Tuan = g.Key,
                                              TrungBinh = Math.Round(g.Select(c => (c.ProcessedOn - c.CreatedOn).TotalHours).Sum() / g.Count(), 1),
                                              ToiDa = Math.Round(g.Select(c => (c.ProcessedOn - c.CreatedOn).TotalHours).Max(), 1),
                                              TongSo = g.Count(),
                                          };
                    var labels = rptByTimeValues.Select(x => $"Tuần {x}").ToList();
                    return new APIResult<DashboardViewModel>
                    {
                        Data = new DashboardViewModel
                        {
                            Labels = labels,
                            Datasets = new List<LineChartModel>() {
                                new LineChartModel
                                {
                                    Label = "Thời gian xử lý trung bình",
                                    Data = rptByTimeValues.Select(x => x.TrungBinh),
                                    BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Take(1),
                                    BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Take(1),
                                    Tension = 0.4,
                                },
                                new LineChartModel
                                {
                                    Label = "Thời gian xử lý tối đa",
                                    Data = rptByTimeValues.Select(x => x.ToiDa),
                                    BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Skip(1).Take(1),
                                    BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Skip(1).Take(1),
                                    Tension = 0.4,
                                },
                            },
                        }
                    };
                case 3: //equal 0,1
                    #region Số lượng YC phát sinh theo ngày
                    ticketTypes = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new Criteria.CodeDetailCriteria { Master = CodeDetailMaster.TicketType }) ?? new List<CodeDetail>();
                    var grpRptValues = ticketsInMonth!.GroupBy(x => new { x.CreatedOn.Day, x.Type }).Select(x => new { x.Key.Day, x.Key.Type, Count = x.Count() });
                    int[] days = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToArray();
                    var rptByIssueValues = from x in ticketsInMonth
                                           join t in ticketTypes on x.Type equals t.Code
                                           group x by t.Description into g
                                           orderby g.Key
                                           select new
                                           {
                                               Label = g.Key,
                                               Total = g.Count(),
                                           };
                    labels = days.Select(x => $"Ngày {x}").ToList();
                    var rptType2 = new DashboardViewModel
                    {
                        Labels = labels,
                        Datasets = from tt in ticketTypes
                                   select new LineChartModel
                                   {
                                       Label = tt.Description,
                                       Data = from d in days
                                              join v in grpRptValues on new { Day = d, Type = tt.Code } equals new { v.Day, v.Type } into leftv
                                              from v in leftv.DefaultIfEmpty()
                                              select Convert.ToDouble(v?.Count ?? 0),
                                       //BorderColor = Globals.ChartFormats.Select(x => x.BorderColor).Take(labels.Count()),
                                       //BackgroundColor = Globals.ChartFormats.Select(x => x.BackgroundColor).Take(labels.Count()),
                                       Tension = 0.4,
                                       BorderWidth = 0.5,
                                   }
                    };
                    #endregion Số lượng YC phát sinh theo ngày

                    #region Số lượng YC chưa được xử trí
                    criteria = new Criteria.TicketCriteria
                    {
                        ExcludeStatus = new List<TicketStatus> { TicketStatus.Closed, TicketStatus.Done }
                    };
                    var unFinishedTickets = await HPDQ.WebSupport.Utilities.API.Instance.Ticket.Load(criteria);
                    unFinishedTickets = unFinishedTickets ?? new List<Ticket>();

                    var rptValues = from x in ticketsInMonth
                                    group x by x.CreatedOn.Day into g
                                    select new
                                    {
                                        Date = g.Key,
                                        Total = g.Count(),
                                        Processed = g.Count(x => x.Status == TicketStatus.Done || x.Status == TicketStatus.Closed),
                                        InProgress = g.Count(x => x.Status == TicketStatus.InProgress),
                                    };
                    labels = ticketTypes.Select(x => x.Description).OrderBy(x => x).ToList();
                    var inProgress = rptValues.Sum(x => x.InProgress);
                    var processed = rptValues.Sum(x => x.Processed);
                    var rptType4 = new DashboardViewModel
                    {
                        Labels = labels,
                        Datasets = new List<LineChartModel>() {
                            new LineChartModel
                            {
                                Label = "Số lượng yêu cầu",
                                Data = from tt in ticketTypes
                                       join t in unFinishedTickets!.GroupBy(x=>x.Type) on tt.Code equals t.Key into lt
                                       from t in lt.DefaultIfEmpty()
                                       select t?.Count() ?? 0d,
                                Tension = 0.4,
                            },
                        },
                    };
                    #endregion Số lượng YC chưa được xử trí

                    return new APIResult<IEnumerable<DashboardViewModel>>
                    {
                        Data = new List<DashboardViewModel>
                        {
                            RptType0(ticketsInMonth,ticketTypes),
                            RptType1(ticketsInMonth),
                            rptType2,
                            rptType4,
                        }
                    };
                default:
                    return new APIResult<DashboardViewModel> { };
            }
        }
    }
}