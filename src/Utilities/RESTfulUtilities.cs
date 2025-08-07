using System.Text.Json;

namespace Core.RESTful
{
    /// <summary>
    /// Cung cấp các phương thức tiện ích để thực hiện các truy xuất qua API RESTful.
    /// </summary>
    /// <remarks>
    /// Lớp này sử dụng HttpClient để gửi các yêu cầu HTTP (GET, POST) và xử lý phản hồi JSON.
    /// Nó hỗ trợ cấu hình Base URL và thêm header xác thực.
    /// </remarks>
    public class RESTfulUtilities
    {
        private HttpClient _client = new HttpClient();

        /// <summary>
        /// Cấu hình địa chỉ của API cho một đối tượng <see cref="RESTfulUtilities"/>.
        /// </summary>
        /// <param name="baseUrl">Địa chỉ của API.</param>
        /// <returns>Một đối tượng của <see cref="RESTfulUtilities"/> để cho phép chuỗi phương thức (method chaining).</returns>
        public RESTfulUtilities SetBaseUrl(string baseUrl)
        {
            if (baseUrl.EndsWith("/") == false) baseUrl += "/";
            _client.BaseAddress = new Uri(baseUrl);
            return this;
        }

        /// <summary>
        /// Cấu hình header xác thực (authentication header) cho tất cả các yêu cầu.
        /// </summary>
        /// <param name="apiKey">API Key để xác thực.</param>
        /// <param name="headerName">Tên của header xác thực. Mặc định là "X-API-KEY".</param>
        /// <returns>Một thể hiện của <see cref="RESTfulUtilities"/> để cho phép chuỗi phương thức.</returns>
        public RESTfulUtilities AddAuthenticationHeader(string apiKey, string headerName = "X-API-KEY")
        {
            _client.DefaultRequestHeaders.Add(headerName, apiKey);
            return this;
        }

        /// <summary>
        /// Thực hiện một yêu cầu HTTP GET bất đồng bộ và phản hồi JSON thành đối tượng.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng trả về.</typeparam>
        /// <param name="url">Địa chỉ truy xuất route action hoặc địa chỉ đầy đủ nếu không có cấu hình <see cref="SetBaseUrl"/> của yêu cầu GET.</param>
        /// <returns>Đối tượng được trả về từ API.</returns>
        public async Task<T?> Get<T>(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return await JsonSerializer.DeserializeAsync<T>(stream, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Utilities at Get with message: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Thực hiện một yêu cầu HTTP POST bất đồng bộ với một đối tượng trong body và phản hồi JSON.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng trả về.</typeparam>
        /// <param name="url">Địa chỉ truy xuất route action hoặc địa chỉ đầy đủ nếu không có cấu hình <see cref="SetBaseUrl"/> của yêu cầu POST.</param>
        /// <param name="body">Đối tượng sẽ được gửi trong body yêu cầu theo định dạng JSON.</param>
        /// <returns>Đối tượng được trả về từ API.</returns>
        public async Task<T?> Post<T>(string url, object body)
        {
            try
            {
                var response = await _client.PostAsJsonAsync(url, body);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx
                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return await JsonSerializer.DeserializeAsync<T>(stream, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Utilities at Get with message: {ex.Message}");
                throw;
            }
        }
    }
}