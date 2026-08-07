using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace IntranetPortal.Base.Models.LeaveModels
{
    public class LeaveResumption
    {
        public long LeaveResumptionId { get; set; }
        public long LeaveRequestId { get; set; }
        public string LeaveEmployeeName { get; set; }
        public DateTime ApprovedResumptionDate { get; set; }
        public DateTime ResumptionDateByEmployee { get; set; }
        public int NoOfExtraDaysByEmployee { get; set; }
        public int NoOfUnusedDaysByEmployee { get; set; }
        public string ReasonByEmployee { get; set; }
        public bool EmployeeRequestAdjustment { get; set; }
        public string RequestedAdjustmentType { get; set; }
        public DateTime DateRecordedByEmployee { get; set; }
        public string LineManagerName { get; set; }
        public DateTime? ResumptionDateByLineManager { get; set; }
        public int NoOfExtraDaysByLineManager { get; set; }
        public int NoOfUnusedDaysByLineManager { get; set; }
        public bool LineManagerApprovesAdjustment { get; set; }
        public string ReasonByLineManager { get; set; }
        public DateTime? DateRecordedByLineManager { get; set; }
    }
}
