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

        [Authorize(Roles = "SystemAdmin")]
        [Route("danhmuc")]
        public async Task<IActionResult> MasterData(int enumVal = 1)
        {
            var datas = await HPDQ.WebSupport.Utilities.API.Instance.CodeDetail.Load(new Criteria.CodeDetailCriteria { Master = (HPDQ.WebSupport.DataEntitites.CodeDetailMaster?)enumVal }) ?? new List<DataEntitites.CodeDetail>();
            return View(datas);
        }

        [Authorize(Roles = "SystemAdmin")]
        [Route("nhanvien")]
        public async Task<IActionResult> Delegation()
        {
            var datas = await HPDQ.WebSupport.Utilities.API.Instance.Employee.Load();
            return View(datas);
        }
    }
}