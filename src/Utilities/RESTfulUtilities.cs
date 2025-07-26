using System.Text.Json;

namespace Core.RESTful
{
    public class RESTfulUtilities
    {
        private HttpClient _client = new HttpClient();
        public RESTfulUtilities SetBaseUrl(string baseUrl)
        {
            if (baseUrl.EndsWith("/") == false) baseUrl += "/";
            _client.BaseAddress = new Uri(baseUrl);
            return this;
        }
        public RESTfulUtilities AddAuthenticationHeader(string apiKey, string headerName = "X-API-KEY")
        {
            _client.DefaultRequestHeaders.Add(headerName, apiKey);
            return this;
        }
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