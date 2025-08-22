using IntranetPortal.Base.Models.PartnerServicesModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PartnerServices.Models
{
    public class BusinessesListViewModel:BaseListViewModel
    {
        public string cn { get; set; }
        public List<Business> BusinessList { get; set; }
    }
}
