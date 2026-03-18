using IntranetPortal.Base.Models.GlobalSettingsModels;
using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class MyServiceRequestsListViewModel:BaseListViewModel
    {
        public DateTime? sd { get; set; }
        public DateTime? ed { get; set; }
        public string RequestOwnerID { get; set; }
        public string RequestOwnerName { get; set; }
        public List<ServiceIncident> ServiceIncentsList { get; set; }
        public List<Team> TeamList { get; set; }

    }
}
