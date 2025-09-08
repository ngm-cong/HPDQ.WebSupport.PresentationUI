using HPDQ.WebSupport.Utilities.Models;

namespace HPDQ.WebSupport.Models
{
    /// <summary>
    /// ViewModel được sử dụng trên trang chủ để hiển thị thông tin Ticket.
    /// </summary>
    /// <remarks>
    /// Lớp này kế thừa từ TicketAPIModel và bổ sung thêm các thuộc tính 
    /// dành riêng cho giao diện người dùng, giúp hiển thị thông tin đầy đủ và thân thiện hơn.
    /// </remarks>
    public class HomeViewModel : TicketAPIModel
    {
        /// <summary>
        /// Mô tả chi tiết của loại lỗi của Ticket (ví dụ: "Yêu cầu kỹ thuật").
        /// </summary>
        public string? TypeDescription { get; set; }
    }
}