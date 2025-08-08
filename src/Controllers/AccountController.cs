using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HPDQ.WebSupport.Controllers
{
    /// <summary>
    /// Controller xử lý các tác vụ liên quan đến tài khoản người dùng, bao gồm đăng nhập, đăng xuất và từ chối truy cập.
    /// </summary>
    /// <remarks>
    /// Controller này sử dụng cookie authentication để xác thực người dùng. Nó cung cấp các action
    /// để hiển thị trang đăng nhập, xử lý yêu cầu đăng nhập, và đăng xuất người dùng khỏi hệ thống.
    /// </remarks>
    public class AccountController : Controller
    {
        /// <summary>
        /// Hiển thị trang đăng nhập.
        /// </summary>
        /// <returns>Một View cho phép người dùng nhập thông tin đăng nhập.</returns>
        [HttpGet]
        public IActionResult Login() => View();

        /// <summary>
        /// Hiển thị trang báo lỗi khi người dùng không có quyền truy cập.
        /// </summary>
        /// <returns>Một View thông báo về việc từ chối truy cập.</returns>
        [HttpGet]
        public IActionResult AccessDenied() => View();

        /// <summary>
        /// Xử lý yêu cầu đăng nhập từ người dùng.
        /// </summary>
        /// <param name="emp_id">Mã của nhân viên.</param>
        /// <param name="password">Mật khẩu của người dùng.</param>
        /// <returns>
        /// Redirect đến trang chủ nếu đăng nhập thành công, ngược lại trả về View với thông báo lỗi.
        /// </returns>
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

        /// <summary>
        /// Đăng xuất người dùng hiện tại.
        /// </summary>
        /// <returns>
        /// Redirect người dùng về trang đăng nhập sau khi đã đăng xuất.
        /// </returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }
    }
}