using IntranetPortal.Base.Models.BaseModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PartnerServices.Models
{
    public class ContactListViewModel : BaseListViewModel
    {
        public List<BusinessContact> ContactsList { get; set; }
    }
}
