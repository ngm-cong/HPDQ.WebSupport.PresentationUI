using HPDQ.WebSupport.Criteria;
using HPDQ.WebSupport.DataEntitites;

namespace WebSupport.Utilities
{
    public class API
    {
        public static API Instance = new API();
        public APIRoute Ticket { get; set; } = new APIRoute();
        public API()
        {
            var restfulCore = (new Core.RESTful.RESTfulUtilities()).AddAuthenticationHeader(WebSupport.Utilities.Globals.APIKey!)
                .SetBaseUrl(WebSupport.Utilities.Globals.APIUrl!);
            Ticket.Configs(restfulCore, "Tickets");
        }
        public class APIRoute : APICore
        {
            public async Task<IEnumerable<Ticket>?> Load(TicketCriteria criteria)
            {
                var retVal = await routeAPI.Post<APIResult<IEnumerable<Ticket>>>("Load", criteria);
                if (retVal != null && retVal.ErrorCode == 0 && retVal.Data != null) return retVal.Data;
                return null;
            }
        }
    }
}
