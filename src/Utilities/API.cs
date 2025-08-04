using HPDQ.WebSupport.Criteria;
using HPDQ.WebSupport.DataEntitites;

namespace WebSupport.Utilities
{
    public class API
    {
        public static API Instance = new API();
        public TicketRoute Ticket { get; set; } = new TicketRoute();
        public CodeDetailRoute CodeDetail { get; set; } = new CodeDetailRoute();
        public API()
        {
            var restfulCore = (new Core.RESTful.RESTfulUtilities()).AddAuthenticationHeader(WebSupport.Utilities.Globals.APIKey!)
                .SetBaseUrl(WebSupport.Utilities.Globals.APIUrl!);
            Ticket.Configs(restfulCore, "Tickets");
            CodeDetail.Configs(restfulCore, "CodeDetails");
        }
        public class TicketRoute : Utilities.APIRoute
        {
            public async Task<IEnumerable<Ticket>?> Load(TicketCriteria criteria)
            {
                var retVal = await routeAPI.Post<APIResult<IEnumerable<Ticket>>>("Load", criteria);
                if (retVal != null && retVal.ErrorCode == 0 && retVal.Data != null) return retVal.Data;
                return null;
            }
        }
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
