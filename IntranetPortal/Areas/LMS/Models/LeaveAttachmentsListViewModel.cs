using IntranetPortal.Base.Models.LmsModels;
using IntranetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetPortal.Areas.LMS.Models
{
    public class LeaveAttachmentsListViewModel:BaseListViewModel
    {
        public long LeaveId { get; set; }
        public List<LeaveDocument> AttachmentList { get; set; }
    }
}
