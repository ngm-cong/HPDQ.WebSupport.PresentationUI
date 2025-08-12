/// <summary>
/// Middleware toàn cục để xử lý các ngoại lệ (exception) không được xử lý.
/// </summary>
/// <remarks>
/// Middleware này được đặt ở đầu pipeline yêu cầu HTTP để bắt các ngoại lệ
/// từ các middleware phía sau. Nó sẽ ghi lại ngoại lệ, trả về một phản hồi JSON
/// chuẩn hóa với mã lỗi và thông báo lỗi.
/// </remarks>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>
    /// Khởi tạo một thể hiện mới của GlobalExceptionMiddleware.
    /// </summary>
    /// <param name="next">Delegate sẽ được gọi để chuyển yêu cầu đến middleware tiếp theo.</param>
    /// <param name="logger">Logger để ghi lại các lỗi.</param>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý yêu cầu HTTP và bắt các ngoại lệ không được xử lý.
    /// </summary>
    /// <param name="context">HttpContext của yêu cầu hiện tại.</param>
    /// <returns>Một Task đại diện cho hoạt động bất đồng bộ.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Continue pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            // Hủy bỏ trạng thái phản hồi hiện tại
            context.Response.Clear();

            // Đặt trạng thái phản hồi lỗi
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // Chuyển hướng tới trang lỗi mặc định của ASP.NET Core
            // Tùy thuộc vào cấu hình, trang lỗi có thể là /Home/Error hoặc /Error
            // Lệnh này đảm bảo middleware khác trong pipeline có thể xử lý lỗi
            context.Response.Redirect("/Home/Error");
        }
    }
}