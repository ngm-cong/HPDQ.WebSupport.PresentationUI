using HPDQ.WebSupport.DataEntitites;
using System.Data;

namespace HPDQ.WebSupport.Models
{
    /// <summary>
    /// ViewModel được sử dụng trên giao diện người dùng để hiển thị báo cáo.
    /// </summary>
    /// <remarks>
    /// Lớp này đóng gói dữ liệu cần thiết cho trang báo cáo, bao gồm danh sách các loại báo cáo có sẵn và dữ liệu báo cáo thực tế dưới dạng DataTable.
    /// </remarks>
    public class ReportViewModel
    {
        /// <summary>
        /// Danh sách các loại báo cáo có sẵn, dùng để hiển thị trong các control chọn lựa.
        /// </summary>
        public IEnumerable<ReportType>? ReportTypes { get; set; }

        /// <summary>
        /// Dữ liệu của báo cáo dưới dạng bảng, dùng để hiển thị kết quả truy vấn dữ liệu báo cáo trên giao diện.
        /// </summary>
        public DataTable? DataTable { get; set; }
    }
}