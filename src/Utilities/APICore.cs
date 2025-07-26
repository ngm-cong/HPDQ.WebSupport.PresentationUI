namespace WebSupport.Utilities
{
    public class APICore
    {
        internal RouteAPI routeAPI = new RouteAPI();
        public void Configs(Core.RESTful.RESTfulUtilities restfulCore, string routeUrl)
        {
            routeAPI.Configs(restfulCore, routeUrl);
        }
    }
    public class RouteAPI
    {
        private Core.RESTful.RESTfulUtilities? _restfulCore;
        private string? _routeUrl;
        public void Configs(Core.RESTful.RESTfulUtilities restfulCore, string routeUrl)
        {
            _restfulCore = restfulCore;
            _routeUrl = routeUrl;
        }
        public async Task<T?> Get<T>(string url)
        {
            return await _restfulCore!.Get<T>($"/{_routeUrl}/{url}");
        }
        public async Task<T?> Post<T>(string url, object body)
        {
            return await _restfulCore!.Post<T>($"/{_routeUrl}/{url}", body);
        }
    }
}
