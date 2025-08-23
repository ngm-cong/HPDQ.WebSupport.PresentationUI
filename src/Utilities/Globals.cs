namespace HPDQ.WebSupport.Utilities
{
    /// <summary>Cấu trúc đối tượng đại diện cho dữ liệu cấu hình cho các chart hiển thị.</summary>
    public class ChartFormat
    {
        /// <summary>Dữ liệu cài đặt màu viền cho một dữ liệu chart.</summary>
        public string BorderColor { get; set; } = string.Empty;
        /// <summary>Dữ liệu cài đặt màu nền cho một dữ liệu chart.</summary>
        public string BackgroundColor { get; set; } = string.Empty;
    }

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

        /// <summary>
        /// Địa chỉ của API dạng domain dành cho các truy xuất API bằng ajax client-side.
        /// </summary>
        public static string? DomainAPIUrl { get; set; }

        /// <summary>Địa chỉ của dịch vụ SignalR.</summary>
        public static string? SignalRUrl { get; set; }

        /// <summary>Địa chỉ của API của dịch vụ đăng nhập.</summary>
        public static string? AuthenticateAPIUrl { get; set; }

        /// <summary>Danh sách các cài đặt dữ liệu trên chart sắp xếp theo thứ tự.</summary>
        public static IEnumerable<ChartFormat> ChartFormats = new List<ChartFormat>
        {
            new ChartFormat { BorderColor = "red", BackgroundColor = "#45adeb" },
            new ChartFormat { BorderColor = "blue", BackgroundColor = "#ff809a" },
            new ChartFormat { BorderColor = "green", BackgroundColor = "#ffa500" },
            new ChartFormat { BorderColor = "orange", BackgroundColor = "rgba(255,165,0,0.1)" },
            new ChartFormat { BorderColor = "purple", BackgroundColor = "rgba(128,0,128,0.1)" },
            new ChartFormat { BorderColor = "cyan", BackgroundColor = "rgba(0,255,255,0.1)" },
            new ChartFormat { BorderColor = "magenta", BackgroundColor = "rgba(255,0,255,0.1)" },
            new ChartFormat { BorderColor = "yellow", BackgroundColor = "rgba(255,255,0,0.1)" },
            new ChartFormat { BorderColor = "brown", BackgroundColor = "rgba(165,42,42,0.1)" },
            new ChartFormat { BorderColor = "teal", BackgroundColor = "rgba(0,128,128,0.1)" },
            new ChartFormat { BorderColor = "navy", BackgroundColor = "rgba(0,0,128,0.1)" },
            new ChartFormat { BorderColor = "maroon", BackgroundColor = "rgba(128,0,0,0.1)" },
            new ChartFormat { BorderColor = "olive", BackgroundColor = "rgba(128,128,0,0.1)" },
            new ChartFormat { BorderColor = "lime", BackgroundColor = "rgba(0,255,0,0.1)" },
            new ChartFormat { BorderColor = "indigo", BackgroundColor = "rgba(75,0,130,0.1)" },
            new ChartFormat { BorderColor = "pink", BackgroundColor = "rgba(255,192,203,0.1)" },
            new ChartFormat { BorderColor = "gold", BackgroundColor = "rgba(255,215,0,0.1)" },
            new ChartFormat { BorderColor = "silver", BackgroundColor = "rgba(192,192,192,0.1)" },
            new ChartFormat { BorderColor = "gray", BackgroundColor = "rgba(128,128,128,0.1)" },
            new ChartFormat { BorderColor = "black", BackgroundColor = "rgba(0,0,0,0.1)" },
            new ChartFormat { BorderColor = "#1abc9c", BackgroundColor = "rgba(26,188,156,0.1)" },
            new ChartFormat { BorderColor = "#2ecc71", BackgroundColor = "rgba(46,204,113,0.1)" },
            new ChartFormat { BorderColor = "#3498db", BackgroundColor = "rgba(52,152,219,0.1)" },
            new ChartFormat { BorderColor = "#9b59b6", BackgroundColor = "rgba(155,89,182,0.1)" },
            new ChartFormat { BorderColor = "#34495e", BackgroundColor = "rgba(52,73,94,0.1)" },
            new ChartFormat { BorderColor = "#f1c40f", BackgroundColor = "rgba(241,196,15,0.1)" },
            new ChartFormat { BorderColor = "#e67e22", BackgroundColor = "rgba(230,126,34,0.1)" },
            new ChartFormat { BorderColor = "#e74c3c", BackgroundColor = "rgba(231,76,60,0.1)" },
            new ChartFormat { BorderColor = "#ecf0f1", BackgroundColor = "rgba(236,240,241,0.1)" },
            new ChartFormat { BorderColor = "#95a5a6", BackgroundColor = "rgba(149,165,166,0.1)" },
            new ChartFormat { BorderColor = "#d35400", BackgroundColor = "rgba(211,84,0,0.1)" },
            new ChartFormat { BorderColor = "#c0392b", BackgroundColor = "rgba(192,57,43,0.1)" },
            new ChartFormat { BorderColor = "#2980b9", BackgroundColor = "rgba(41,128,185,0.1)" },
            new ChartFormat { BorderColor = "#16a085", BackgroundColor = "rgba(22,160,133,0.1)" },
            new ChartFormat { BorderColor = "#27ae60", BackgroundColor = "rgba(39,174,96,0.1)" },
            new ChartFormat { BorderColor = "#8e44ad", BackgroundColor = "rgba(142,68,173,0.1)" },
            new ChartFormat { BorderColor = "#2c3e50", BackgroundColor = "rgba(44,62,80,0.1)" },
            new ChartFormat { BorderColor = "#bdc3c7", BackgroundColor = "rgba(189,195,199,0.1)" },
            new ChartFormat { BorderColor = "#7f8c8d", BackgroundColor = "rgba(127,140,141,0.1)" }
        };
    }
}