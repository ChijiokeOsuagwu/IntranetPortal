using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.SrmModels
{
    public class ServiceRequestActivity
    {
        public long ActivityHistoryId { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime? ActivityTime { get; set; }
        public string ActivityBy { get; set; }
        public long ServiceIncidentId { get; set; }
    }
}
