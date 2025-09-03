using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;

// ✳️ Cấu hình logger từ rất sớm (trước khi builder được tạo)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()   // ⚠️ Chỉ định mức tối thiểu là Error
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt"
        , restrictedToMinimumLevel: LogEventLevel.Error // Chỉ ghi lỗi
        , rollingInterval: RollingInterval.Day
        , shared: true
    )
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    // ✅ Gán Serilog cho Host ngay sau builder được tạo
    builder.Host.UseSerilog();

    // Gán giá trị APIKey và APIUrl từ file cấu hình (ví dụ: appsettings.json) vào biến toàn cục trong Globals.
    HPDQ.WebSupport.Utilities.Globals.APIKey = builder.Configuration[$"APISettings:APIKey"];
    HPDQ.WebSupport.Utilities.Globals.APIUrl = builder.Configuration[$"APISettings:APIUrl"];
    HPDQ.WebSupport.Utilities.Globals.DomainAPIUrl = builder.Configuration[$"APISettings:DomainAPIUrl"] ?? HPDQ.WebSupport.Utilities.Globals.APIUrl;
    HPDQ.WebSupport.Utilities.Globals.SignalRUrl = builder.Configuration[$"APISettings:SignalRUrl"];
    HPDQ.WebSupport.Utilities.Globals.AuthenticateAPIUrl = builder.Configuration[$"APISettings:AuthenticateAPIUrl"];

    // Thêm dịch vụ nén phản hồi & Cấu hình các tùy chọn cho Gzip
    builder.Services.AddResponseCompression(options =>
    {
        // Chỉ nén các kiểu dữ liệu cụ thể
        options.EnableForHttps = true;
        options.Providers.Add<GzipCompressionProvider>();
    }).Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Optimal;
    });

    // Thêm dịch vụ hỗ trợ Controller và View cho mô hình MVC.
    builder.Services.AddControllersWithViews();

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"));
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

    // Sử dụng middleware nén phản hồi
    app.UseResponseCompression();
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
    // Đăng ký middleware xử lý lỗi toàn cục.
    app.UseMiddleware<GlobalExceptionMiddleware>();
    // Định nghĩa một route mặc định cho ứng dụng MVC.
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    Core.RESTful.Utilities.RegisterAPI<HPDQ.WebSupport.Utilities.API>(() => (new Core.RESTful.RESTfulUtilities()).AddAuthenticationHeader(HPDQ.WebSupport.Utilities.Globals.APIKey!)
            .SetBaseUrl(HPDQ.WebSupport.Utilities.Globals.APIUrl!));
    Core.RESTful.Utilities.RegisterAPI<HPDQ.WebSupport.Utilities.SignalRAPI>(() => (new Core.RESTful.RESTfulUtilities()).SetBaseUrl(HPDQ.WebSupport.Utilities.Globals.SignalRUrl!));
    Core.RESTful.Utilities.RegisterAPI<HPDQ.WebSupport.Utilities.AuthenticateAPI>(() => (new Core.RESTful.RESTfulUtilities()).SetBaseUrl(HPDQ.WebSupport.Utilities.Globals.AuthenticateAPIUrl!));

    // Chạy ứng dụng web.
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
// dotnet remove package HPDQ.WebSupport.DataEntitites
// dotnet add reference "../../1. HPDQ.WebSupport.DataEntitites/src/HPDQ.WebSupport.DataEntitites.csproj"