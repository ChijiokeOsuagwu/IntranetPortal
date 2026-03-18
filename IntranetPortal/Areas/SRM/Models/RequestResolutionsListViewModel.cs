using IntranetPortal.Base.Models.SrmModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.SRM.Models
{
    public class RequestResolutionsListViewModel:BaseListViewModel
    {
        public long rd { get; set; }
        public List<IncidentResolution> IncidentResolutionsList { get; set; }
    }
}
