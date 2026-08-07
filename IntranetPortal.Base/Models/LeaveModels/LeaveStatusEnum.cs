using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public enum LeaveStatusEnum
    {
        Proposal,
        PendingApproval,
        Declined,
        Approved,
        Confirmed,
        ResumptionNotice,
        PendingClosure,
        Cancelled,
        Completed
    }
}
