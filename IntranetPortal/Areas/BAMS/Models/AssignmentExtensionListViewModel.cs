using IntranetPortal.Base.Models.BamsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.BAMS.Models
{
    public class AssignmentExtensionListViewModel : BaseListViewModel
    {
        public int AssignmentEventID { get; set; }
        public List<AssignmentExtension> AssignmentExtensionList { get; set; }
    }
}
