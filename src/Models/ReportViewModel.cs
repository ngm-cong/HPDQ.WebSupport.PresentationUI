using HPDQ.WebSupport.DataEntitites;
using System.Data;

namespace HPDQ.WebSupport.Models
{
    public class ReportViewModel
    {
        public IEnumerable<ReportType>? ReportTypes { get; set; }
        public DataTable? DataTable { get; set; }
    }
}