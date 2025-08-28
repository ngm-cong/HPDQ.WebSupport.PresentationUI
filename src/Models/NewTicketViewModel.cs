using HPDQ.WebSupport.DataEntitites;

namespace HPDQ.WebSupport.Models
{
    public class NewTicketViewModel
    {
        public IEnumerable<CodeDetail>? TicketTypes { get; set; }
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public byte Type { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}