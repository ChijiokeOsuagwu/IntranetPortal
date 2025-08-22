using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PartnerServices.Models
{
    public class BusinessSectorsListViewModel:BaseListViewModel
    {
        public List<IndustrySector> IndustrySectorsList { get; set; }
    }
}
