using IntranetPortal.Base.Models.LeaveModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LVM.Models
{
    public class LeaveDocumentListViewModel:BaseListViewModel
    {
        public long LeaveRequestId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public string LeaveTypeName { get; set; }
        public long? SubmissionId { get; set; }
        public List<LeaveDocument> LeaveDocumentList { get; set; }
    }
}
