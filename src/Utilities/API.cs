using HPDQ.WebSupport.Criteria;
using HPDQ.WebSupport.DataEntitites;

namespace HPDQ.WebSupport.Utilities
{
    /// <summary>
    /// Cung cấp các phương thức để tương tác với API của hệ thống hỗ trợ.
    /// </summary>
    /// <remarks>
    /// Lớp này được triển khai theo mô hình Singleton để đảm bảo chỉ có một đối tượng duy nhất được tạo ra.
    /// Nó quản lý các đối tượng APIRoute để truy cập các tài nguyên (resource) như Ticket và CodeDetail.
    /// </remarks>
    public class API
    {
        /// <summary>
        /// Đối tượng Singleton của lớp API.
        /// </summary>
        public static API Instance = new API();

        /// <summary>
        /// Đối tượng cung cấp các phương thức để truy cập các API liên quan đến yêu cầu.
        /// </summary>
        public TicketRoute Ticket { get; set; } = new TicketRoute();

        /// <summary>
        /// Đối tượng cung cấp các phương thức để truy cập các API liên quan đến các dữ liệu định nghĩa sẵn.
        /// </summary>
        public CodeDetailRoute CodeDetail { get; set; } = new CodeDetailRoute();

        /// <summary>
        /// Khởi tạo một đối tượng mới của lớp API.
        /// </summary>
        /// <remarks>
        /// Constructor này thiết lập cấu hình cho các route API, bao gồm URL cơ sở và API Key.
        /// </remarks>
        public API()
        {
            var restfulCore = (new Core.RESTful.RESTfulUtilities()).AddAuthenticationHeader(HPDQ.WebSupport.Utilities.Globals.APIKey!)
                .SetBaseUrl(HPDQ.WebSupport.Utilities.Globals.APIUrl!);
            Ticket.Configs(restfulCore, "Tickets");
            CodeDetail.Configs(restfulCore, "CodeDetails");
        }

        /// <summary>
        /// Cung cấp các phương thức để tương tác với tài nguyên yêu cầu thông qua API.
        /// </summary>
        public class TicketRoute : Utilities.APIRoute
        {
            public async Task<IEnumerable<Ticket>?> Load(TicketCriteria criteria)
            {
                var retVal = await routeAPI.Post<APIResult<IEnumerable<Ticket>>>("Load", criteria);
                if (retVal != null && retVal.ErrorCode == 0 && retVal.Data != null) return retVal.Data;
                return null;
            }
        }

        /// <summary>
        /// Cung cấp các phương thức để tương tác với tài nguyên dữ liệu định nghĩa sẵn thông qua API.
        /// </summary>
        public class CodeDetailRoute : Utilities.APIRoute
        {
            public async Task<IEnumerable<CodeDetail>?> Load(CodeDetailCriteria criteria)
            {
                var retVal = await routeAPI.Post<APIResult<IEnumerable<CodeDetail>>>("Load", criteria);
                if (retVal != null && retVal.ErrorCode == 0 && retVal.Data != null) return retVal.Data;
                return null;
            }
        }
    }
}
