namespace WebSupport.Utilities
{
    /// <summary>
    /// Cung cấp một lớp cơ sở để định tuyến và tương tác với các endpoint API cụ thể.
    /// </summary>
    /// <remarks>
    /// Lớp này đóng vai trò như một proxy, sử dụng một thể hiện của <see cref="APICore"/>
    /// để thực hiện các lời gọi API sau khi đã được cấu hình.
    /// </remarks>
    public class APIRoute
    {
        internal APICore routeAPI = new APICore();

        /// <summary>
        /// Cấu hình APIRoute với các thiết lập API chung và URL cụ thể.
        /// </summary>
        /// <param name="restfulCore">Đối tượng của <see cref="Core.RESTful.RESTfulUtilities"/> sẽ được inject vào đối tượng <see cref="APICore"/>.</param>
        /// <param name="routeUrl">Phần URL của route API.</param>
        public void Configs(Core.RESTful.RESTfulUtilities restfulCore, string routeUrl)
        {
            routeAPI.Configs(restfulCore, routeUrl);
        }
    }

    /// <summary>
    /// Lõi để thực hiện các yêu cầu HTTP đến một route API đã được định nghĩa.
    /// </summary>
    /// <remarks>
    /// Lớp này inject một đối tượng của <see cref="Core.RESTful.RESTfulUtilities"/> với
    /// một URL route cụ thể để đơn giản hóa việc gửi các yêu cầu GET và POST.
    /// </remarks>
    public class APICore
    {
        private Core.RESTful.RESTfulUtilities? _restfulCore;
        private string? _routeUrl;

        /// <summary>
        /// Cấu hình <see cref="APICore"/> với một đối tượng của <see cref="Core.RESTful.RESTfulUtilities"/> sẽ được inject và URL route cụ thể.
        /// </summary>
        /// <param name="restfulCore">Đối tượng <see cref="Core.RESTful.RESTfulUtilities"/> sẽ được inject.</param>
        /// <param name="routeUrl">Phần URL của route API (ví dụ: "Tickets").</param>
        public void Configs(Core.RESTful.RESTfulUtilities restfulCore, string routeUrl)
        {
            _restfulCore = restfulCore;
            _routeUrl = routeUrl;
        }

        /// <summary>
        /// Thực hiện một yêu cầu HTTP GET bất đồng bộ trên route đã cấu hình.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng trả về.</typeparam>
        /// <param name="url">URL action của yêu cầu GET.</param>
        /// <returns>Đối tượng được trả về từ kết quả API.</returns>
        public async Task<T?> Get<T>(string url)
        {
            return await _restfulCore!.Get<T>($"/{_routeUrl}/{url}");
        }

        /// <summary>
        /// Thực hiện một yêu cầu HTTP POST bất đồng bộ trên route đã cấu hình.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng trả về.</typeparam>
        /// <param name="url">URL action của yêu cầu POST.</param>
        /// <param name="body">Đối tượng sẽ gửi trong body yêu cầu.</param>
        /// <returns>Đối tượng được trả về từ kết quả API.</returns>
        public async Task<T?> Post<T>(string url, object body)
        {
            return await _restfulCore!.Post<T>($"/{_routeUrl}/{url}", body);
        }
    }
}
