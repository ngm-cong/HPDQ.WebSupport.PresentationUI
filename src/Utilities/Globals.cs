namespace HPDQ.WebSupport.Utilities
{
    /// <summary>
    /// Chứa các biến, hàm toàn cục được sử dụng trong ứng dụng.
    /// </summary>
    /// <remarks>
    /// Lớp này được sử dụng để lưu trữ các giá trị cấu hình chung, các hàm dùng chung
    /// có thể truy cập từ bất kỳ đâu trong ứng dụng.
    /// </remarks>
    public class Globals
    {
        /// <summary>
        /// API Key được sử dụng để xác thực khi gọi các API.
        /// </summary>
        public static string? APIKey { get; set; }

        /// <summary>
        /// Địa chỉ của API.
        /// </summary>
        public static string? APIUrl { get; set; }
    }
}