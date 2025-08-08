using HPDQ.WebSupport.DataEntitites;

namespace HPDQ.WebSupport.Models
{
    public class NewTicketViewModel
    {
        public IEnumerable<CodeDetail>? TicketTypes { get; set; }
    }
}