using HPDQ.WebSupport.DataEntitites;

namespace HPDQ.WebSupport.Models
{
    /// <summary>
    /// ViewModel được sử dụng trên giao diện tạo Ticket mới.
    /// </summary>
    /// <remarks>
    /// Lớp này đóng gói dữ liệu cần thiết để hiển thị form tạo Ticket,
    /// bao gồm danh sách các loại Ticket có sẵn và thông tin chi tiết của Ticket đang được tạo.
    /// </remarks>
    public class NewTicketViewModel
    {
        /// <summary>
        /// Danh sách các loại lỗi có sẵn, được sử dụng để hiển thị trong một dropdown list.
        /// </summary>
        public IEnumerable<CodeDetail>? TicketTypes { get; set; }

        /// <summary>
        /// ID của Ticket, có thể là 0 nếu đang tạo mới.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mô tả chi tiết của Ticket.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Loại của Ticket (ví dụ: Yêu cầu, Lỗi, v.v.).
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Tiêu đề của Ticket.
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}