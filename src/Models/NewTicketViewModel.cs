using HPDQ.WebSupport.DataEntitites;

namespace WebSupport.Models
{
    public class NewTicketViewModel
    {
        public IEnumerable<CodeDetail>? TicketTypes { get; set; }
    }
}