var builder = WebApplication.CreateBuilder(args);

// Gán giá trị APIKey và APIUrl từ file cấu hình (ví dụ: appsettings.json) vào biến toàn cục trong Globals.
WebSupport.Utilities.Globals.APIKey = builder.Configuration[$"APISettings:APIKey"];
WebSupport.Utilities.Globals.APIUrl = builder.Configuration[$"APISettings:APIUrl"];

// Thêm dịch vụ hỗ trợ Controller và View cho mô hình MVC.
builder.Services.AddControllersWithViews();

// Cấu hình dịch vụ xác thực Cookie-based Authentication.
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "MyAuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

var app = builder.Build();

// Cấu hình pipeline xử lý yêu cầu HTTP.
if (!app.Environment.IsDevelopment()) // Nếu ứng dụng không chạy ở môi trường phát triển (Development), sử dụng trang xử lý lỗi tùy chỉnh.
{
    app.UseExceptionHandler("/Home/Error");
    // Thêm chính sách HSTS (HTTP Strict Transport Security) để bắt buộc sử dụng HTTPS.
    app.UseHsts();
}

// Chuyển hướng các yêu cầu HTTP sang HTTPS.
app.UseHttpsRedirection();
// Cho phép các tệp tĩnh (static files) như CSS, JavaScript, hình ảnh, ...
app.UseStaticFiles();
// Kích hoạt middleware định tuyến (routing) để khớp URL với các endpoint.
app.UseRouting();
// Kích hoạt middleware xác thực (authentication) để nhận diện người dùng.
app.UseAuthentication();
// Kích hoạt middleware phân quyền (authorization) để kiểm tra quyền truy cập.
app.UseAuthorization();
// Định nghĩa một route mặc định cho ứng dụng MVC.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Chạy ứng dụng web.
app.Run();