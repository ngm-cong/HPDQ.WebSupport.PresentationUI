using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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
        /// Lớp chứa thông tin giải mã được của API đăng nhập.
        /// </summary>
        private class AuthenticateModel
        {
            /// <summary>Mã NV</summary>
            public string? EMP_ID { get; set; }
            /// <summary>Họ tên nhân viên</summary>
            public string? FullName { get; set; }
        }

        ///// <summary>Hàm giải mã token thông tin đăng nhập.</summary>
        ///// <param name="jwtToken">Token thông tin đăng nhập</param>
        ///// <returns>Lớp thông tin đăng nhập.</returns>
        ///// <exception cref="Exception"></exception>
        //private AuthenticateModel JWTDecode(string jwtToken)
        //{
        //    // 1. Create a token handler
        //    var handler = new JwtSecurityTokenHandler();

        //    // Check if the token is in a valid format
        //    if (handler.CanReadToken(jwtToken))
        //    {
        //        // 2. Read the token without validating the signature
        //        var token = handler.ReadJwtToken(jwtToken);

        //        // 3. Access the token claims
        //        Console.WriteLine("Token Header:");
        //        Console.WriteLine(token.Header.SerializeToJson());

        //        Console.WriteLine("\nToken Payload (Claims):");
        //        foreach (var claim in token.Claims)
        //        {
        //            Console.WriteLine($"{claim.Type}: {claim.Value}");
        //        }

        //        return new AuthenticateModel { EMP_ID = token.Claims.First(x => x.Type == "MaNV").Value, FullName = token.Claims.First(x => x.Type == "unique_name").Value };
        //    }
        //    else
        //    {
        //        throw new Exception("Invalid JWT format.");
        //    }
        //}

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
            try
            {
                var empToken = await HPDQ.WebSupport.Utilities.AuthenticateAPI.Instance.Authenticate.Login(emp_id, password);

                if (empToken?.success == true && string.IsNullOrEmpty(empToken?.token) == false
                     && string.IsNullOrEmpty(empToken?.hoTen) == false
                     && empToken?.maPhongBan > 0)
                {
                    //var emp = JWTDecode(empToken.token!);

                    // Ví dụ kiểm tra username/password đơn giản
                    var role = "User";
                    if (empToken.maPhongBan == 1961) role = "Admin";
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, emp_id),
                        new Claim(ClaimTypes.GivenName, empToken.hoTen!),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");

                    await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.Error = "Sai thông tin đăng nhập!";
            }
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