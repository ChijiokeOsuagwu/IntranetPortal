using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class ServiceRequestActivitiesViewModel:BaseViewModel
    {
        public long? ServiceIncidentId { get; set; }
        public List<ServiceRequestActivity> ActivityList { get; set; }
    }
}
