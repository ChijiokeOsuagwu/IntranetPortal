using System;
using System.Collections.Generic;
using System.Text;

namespace IntranetPortal.Base.Models.GlobalSettingsModels
{
    public class TeamMember
    {
        public int? TeamMemberID { get; set; }
        public string TeamID { get; set; }
        public string TeamName { get; set; }
        public string MemberID { get; set; }
        public string MemberRole { get; set; }
        public string FullName { get; set; }
        public string EmployeeNo1 { get; set; }
        public string EmployeeNo2 { get; set; }
        public string Sex { get; set; }
        public string PhoneNo { get; set; }
        public string AltPhoneNo { get; set; }
        public string Email { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
