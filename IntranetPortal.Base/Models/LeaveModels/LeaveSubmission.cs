using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveSubmission
    {
        public long LeaveSubmissionId { get; set; }
        public long? LeavePlanId { get; set; }
        public long? LeaveRequestId { get; set; }
        public string DocumentType { get; set; }
        public string FromEmployeeName { get; set; }
        public string ToEmployeeName { get; set; }
        public string ToEmployeeRole { get; set; }
        public string Purpose { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public string Message { get; set; }
        public bool IsActioned { get; set; }
        public DateTime? TimeActioned { get; set; }


        public string LeavePlanEmployeeName { get; set; }
        public string LeavePlanTypeName { get; set; }
        public DateTime LeavePlanStartDate { get; set; }
        public DateTime LeavePlanEndDate { get; set; }
        public DateTime LeavePlanResumptionDate { get; set; }
        public string LeavePlanDurationDescription { get; set; }
        public string LeavePlanStatusDescription { get; set; }
        public int LeavePlanYear { get; set; }
        public string LeavePlanLocationName { get; set; }
        public string LeavePlanUnitName { get; set; }
        

        public string LeaveRequestEmployeeName { get; set; }
        public string LeaveRequestTypeName { get; set; }
        public DateTime LeaveRequestStartDate { get; set; }
        public DateTime LeaveRequestEndDate { get; set; }
        public DateTime LeaveRequestResumptionDate { get; set; }
        public string LeaveRequestDurationDescription { get; set; }
        public string LeaveRequestStatusDescription { get; set; }
        public int LeaveRequestYear { get; set; }
        public string LeaveRequestLocationName { get; set; }
        public string LeaveRequestUnitName { get; set; }
    }
}
