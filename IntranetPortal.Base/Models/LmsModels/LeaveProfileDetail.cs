using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.LmsModels
{
    public class LeaveProfileDetail
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public int Duration { get; set; }
        public int DurationTypeId { get; set; }
        public string DurationTypeDescription { get; set; }
        public bool IsYearly { get; set; }
        public bool CanBeCarriedOver { get; set; }
        public int? CarryOverEndMonth { get; set; }
        public string CarryOverEndMonthName { get; set; }
        public bool CanBeMonetized { get; set; }
    }
}
