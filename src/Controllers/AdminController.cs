using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPDQ.WebSupport.Controllers
{
    /// <summary>
    /// Controller chính để xử lý các yêu cầu liên quan đến quản trị/quản lý danh mục & phân quyền.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        /// <summary>
        /// Khởi tạo một đối tượng mới của <see cref="AdminController"/>.
        /// </summary>
        /// <param name="logger">Logger để ghi lại thông tin.</param>
        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Hiển thị trang quản lý danh mục (Master Data).
        /// </summary>
        /// <remarks>
        /// Endpoint này chỉ có thể được truy cập bởi người dùng có vai trò "SystemAdmin".
        /// Nó tải dữ liệu chi tiết mã (CodeDetail) dựa trên giá trị enum được truyền vào.
        /// </remarks>
        /// <param name="enumVal">Giá trị enum của danh mục cần hiển thị. Mặc định là 1.</param>
        /// <returns>Một view Admin/MasterData.cshtml để hiển thị trang danh mục với dữ liệu tương ứng.</returns>
        [Authorize(Roles = "SystemAdmin")]
        [Route("danhmuc")]
        public async Task<IActionResult> MasterData(int enumVal = 1)
        {
            var datas = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new Criteria.CodeDetailCriteria { Master = (HPDQ.WebSupport.DataEntitites.CodeDetailMaster?)enumVal }) ?? new List<DataEntitites.CodeDetail>();
            return View(datas);
        }

        /// <summary>
        /// Hiển thị trang quản lý nhân viên và phân quyền (Delegation).
        /// </summary>
        /// <remarks>
        /// Endpoint này chỉ có thể được truy cập bởi người dùng có vai trò "SystemAdmin".
        /// Nó tải danh sách tất cả nhân viên để hiển thị trên trang.
        /// </remarks>
        /// <returns>Một view Admin/Delegation.cshtml để hiển thị trang quản lý nhân viên.</returns>
        [Authorize(Roles = "SystemAdmin")]
        [Route("nhanvien")]
        public async Task<IActionResult> Delegation()
        {
            var datas = await HPDQ.WebSupport.Utilities.API.Instance.Employee.Load();
            return View(datas);
        }
    }
}