using IntranetPortal.Base.Models.PmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.PMS.Models
{
    public class AppraisalActivitiesViewModel : BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public string LoggedInEmployeeID { get; set; }
        public int ReviewSessionID { get; set; }
        public string AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }
        public string SourcePage { get; set; }
        public List<PmsActivityHistory> ReviewActivityList { get; set; }
    }

}
